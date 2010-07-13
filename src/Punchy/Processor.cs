using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.Tool;
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
        private readonly Dictionary<string, List<ITool>> toolchains;
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
                throw BundleException.FromFilename(bundlefilename);

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

        private List<ITool> SelectToolchain(IBundle bundle)
        {
            if (this.toolchains.Count == 1 && this.toolchains.ContainsKey("*"))
            {
                return this.toolchains["*"];
            }

            List<ITool> result = null;
            if (!this.toolchains.TryGetValue(bundle.MimeType, out result))
                throw new BundleException("No toolchain found to support bundle type \"" + bundle.MimeType + "\".");

            return result;
        }

        private void ProcessAndSaveBundle(IBundle bundle)
        {
            // Copy each source file to temp location and ensure it exists.
            List<WorkfileContext> info = new List<WorkfileContext>(bundle.FileList.Count);
            foreach (IFile file in bundle.FileList)
            {
                FileInfo fi = new FileInfo(file.PhysicalPath);
                if (!fi.Exists)
                    throw BundleException.FromFilename(file.PhysicalPath);

                info.Add(new WorkfileContext() {
                    Workfile = CopyToTempFile(fi),
                    OriginalSource = fi
                });
            }

            // Run each processor
            foreach (ITool proc in SelectToolchain(bundle))
            {
                proc.Process(new ToolContext()
                {
                    OutputFile = new FileInfo(Path.Combine(this.outputPath, bundle.Filename)),
                    Workfiles = info,
                });
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

            // Populate toolchains
            this.toolchains = new Dictionary<string, List<ITool>>(config.Toolchains.Count);
            foreach (ToolchainElement toolchain in config.Toolchains)
            {
                string key = toolchain.ForMimeType;
                if (String.IsNullOrWhiteSpace(key))
                {
                    if (this.toolchains.ContainsKey("*"))
                        throw new InvalidPunchyConfigurationException("Only one toolchain can be applied to all bundles. For all other toolchains, specify a \"formimetype\" property.");

                    key = "*";
                }

                List<ITool> toolchainList = new List<ITool>(toolchain.Count);
                foreach (ToolElement toolconfig in toolchain)
                {
                    try
                    {
                        ITool instance = toolconfig.Type.CreateInstanceFromTypeString<ITool>();

                        if (instance == null)
                            throw new InvalidPunchyConfigurationException("Tool \"" + toolconfig.Type + "\" is not a valid type string. Use the format \"<fullyqualifiedclassname,assembly>\"");

                        toolchainList.Add(instance);
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new InvalidPunchyConfigurationException("Tool \"" + toolconfig.Type + "\" must implement the Punchy.Tool.ITool interface.", ex);
                    }
                    catch (TypeLoadException ex)
                    {
                        throw new InvalidPunchyConfigurationException("Tool \"" + toolconfig.Type + "\" could not be loaded. Ensure that the assembly it belongs to is in the assembly probing path.", ex);
                    }
                }

                // Make sure the combiner tool is added last.
                toolchainList.Add(new CombinerTool());
                this.toolchains.Add(key, toolchainList);
            }

            this.outputPath = config.OutputPhysicalPath;
            this.outputVirtualPath = config.OutputPath;

            if (!this.outputVirtualPath.EndsWith("/"))
                this.outputVirtualPath = this.outputVirtualPath + "/";

            // Populate bundles
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
