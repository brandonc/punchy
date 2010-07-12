using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.FileProcessor;
using Punchy.Configuration;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Web;

namespace Punchy
{
    public class Processor : IDisposable
    {
        private readonly List<IFileProcessor> processors;
        private readonly Dictionary<string, IBundle> bundles;
        private readonly string outputPath;
        private readonly string tempPath;
        private readonly string outputVirtualPath;
        
        private static int instanceNumber = 0;

        private bool finalized = false;

        public string GetResourceFor(string bundlefilename)
        {
            if(bundlefilename == null)
                throw new ArgumentNullException("bundlefilename");

            IBundle bundle = null;
            if(!this.bundles.TryGetValue(bundlefilename, out bundle))
                throw new BundleException(bundlefilename);

            if (CacheRevisionOutdated(bundle))
            {
                ProcessAndSaveBundle(bundle);
            }

            string physicalPath = Path.Combine(this.outputPath, bundle.Filename);

            if (HttpContext.Current == null)
            {
                return physicalPath;
            }

            return VirtualPathUtility.ToAbsolute(outputVirtualPath + bundlefilename) + "?r=" + bundle.Revision.ToString("X");
        }

        public void Dispose()
        {
            if (!this.finalized)
            {
                finalized = true;
                foreach (string file in Directory.GetFiles(this.tempPath))
                {
                    if (Path.GetFileName(file).StartsWith(String.Format("{0}-{1}-", AppDomain.CurrentDomain.Id, Processor.instanceNumber)))
                    {
                        TryIO(() =>
                        {
                            File.Delete(file);
                        });
                    }
                }
                GC.SuppressFinalize(this);
            }
        }

        ~Processor()
        {
            if (!this.finalized)
            {
                Dispose();
            }
        }

        private string GenerateUniqueTempFilename(string originalFilename)
        {
            // Example: 3-1-jquery-1.4.2-8CCEFEE8D39AA03.js
            //          appid - processor id - orig file name - timestamp(hex).extension

            return String.Format("{0}-{1}-{2}-{3}{4}",
                AppDomain.CurrentDomain.Id,
                Processor.instanceNumber,
                Path.GetFileNameWithoutExtension(originalFilename),
                DateTime.Now.Ticks.ToString("X"),
                Path.GetExtension(originalFilename)
            );
        }

        private FileInfo CopyToTempFile(FileInfo file)
        {
            string tempFile = Path.Combine(this.tempPath, GenerateUniqueTempFilename(file.Name));
            TryIO(() => {
                File.Copy(file.FullName, tempFile);
            });

            return new FileInfo(tempFile);
        }

        private void ProcessAndSaveBundle(IBundle bundle)
        {
            // Copy each source file to temp location and ensure it exists.
            List<FileInfo> info = new List<FileInfo>(bundle.FileList.Count);
            foreach (IFile file in bundle.FileList)
            {
                FileInfo fi = new FileInfo(file.PhysicalPath);
                if (!fi.Exists)
                    throw new BundleException(file.PhysicalPath);

                info.Add(CopyToTempFile(fi));
            }

            // Run each processor
            foreach (IFileProcessor proc in this.processors)
            {
                proc.Process(info);
            }

            // Combine and save into bundle file
            string outputFile = Path.Combine(this.outputPath, bundle.Filename);
            using (StreamWriter writer = new StreamWriter(outputFile, false, Encoding.UTF8, 1024))
            {
                foreach (FileInfo fi in info)
                {
                    using (StreamReader reader = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 1024), true))
                    {
                        char[] buffer = new char[1024];
                        int numRead = 0;
                        while ((numRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, numRead);
                        }
                    }
                }
            }

            bundle.Revision = DateTime.Now.Ticks;
        }

        private bool CacheRevisionOutdated(IBundle bundle)
        {
            return (bundle.Revision <= 0 || AnyBundleFileNewerThanBundle(bundle));
        }

        private bool AnyBundleFileNewerThanBundle(IBundle bundle)
        {
            foreach (IFile file in bundle.FileList)
            {
                FileInfo fi = new FileInfo(file.PhysicalPath);
                if (!fi.Exists || fi.LastWriteTime.Ticks > bundle.Revision)
                {
                    return true;
                }
            }
            return false;
        }

        private void TryIO(Action a)
        {
            try
            {
                a();
            }
            catch (PathTooLongException ex)
            {
                throw new InvalidPunchyConfigurationException("Weird, I know. A path specified in your configuration exceeds the maximum Windows path limit (260).", ex);
            }
            catch (IOException ex)
            {
                throw new InvalidPunchyConfigurationException("Punchy could not create its output directory. Ensure that the folder you have configured is not read only.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidPunchyConfigurationException("Punchy does not have permission to create output files. Check that the output path has the appropriate permissions.", ex);
            }
        }

        public Processor()
        {
            Interlocked.Increment(ref instanceNumber);

            PunchyConfigurationSection config = (PunchyConfigurationSection)ConfigurationManager.GetSection("punchy");

            this.processors = new List<IFileProcessor>(config.Processors.Count);
            this.outputPath = config.OutputPhysicalPath;
            this.outputVirtualPath = config.OutputPath;

            if (!this.outputVirtualPath.EndsWith("/"))
                this.outputVirtualPath = this.outputVirtualPath + "/";

            foreach (ProviderSettings plugin in config.Processors)
            {
                try
                {
                    IFileProcessor proc = plugin.Type.CreateInstanceFromTypeString<IFileProcessor>();
                    if(proc == null)
                        throw new InvalidPunchyConfigurationException("Plugin type string \"" + plugin.Type + "\" was not in the correct format.");

                    this.processors.Add(proc);
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidPunchyConfigurationException("Plugin type \"" + plugin.Type + "\" was not of type \"" + typeof(IFileProcessor).AssemblyQualifiedName + "\".", ex);
                }
            }

            this.bundles = new Dictionary<string, IBundle>(config.Bundles.Count);
            foreach (IBundle bundle in config.Bundles.Cast<IBundle>())
            {
                // Store file revision
                TryIO(() =>
                {
                    FileInfo bundleInfo = new FileInfo(Path.Combine(this.outputPath, bundle.Filename));
                    if (bundleInfo.Exists)
                        bundle.Revision = bundleInfo.LastWriteTime.Ticks;
                });

                this.bundles.Add(bundle.Filename, bundle);
            }

            // Create output/temp folder
            this.tempPath = Path.Combine(this.outputPath, "tmp");

            TryIO(() =>
            {
                if (!Directory.Exists(this.outputPath))
                    Directory.CreateDirectory(this.outputPath);

                if (!Directory.Exists(this.tempPath))
                    Directory.CreateDirectory(this.tempPath);
            });
        }
    }
}
