using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.FileProcessor;
using System.IO;
using dotless.Core;
using dotless.Core.configuration;

namespace Punchy.Plugin.DotLessCss
{
    public class LessCssProcessor : IFileProcessor
    {
        public void Process(ICollection<FileInfo> workspace)
        {
            for(int index = 0; index < workspace.Count; index++)
            {
                FileInfo fi = workspace.Skip(index).Take(1).SingleOrDefault();

                if (Path.GetExtension(fi.Name).ToLower() == ".less")
                {
                    string compressed = null;
                    using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            compressed = Less.Parse(reader.ReadToEnd(), new DotlessConfiguration()
                            {
                                CacheEnabled = false,
                                MinifyOutput = false,
                                Web = false
                            });
                        }
                    }

                    string newFileName = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name)) + ".css";

                    fi.MoveTo(newFileName);

                    using (StreamWriter writer = new StreamWriter(newFileName, false))
                    {
                        writer.Write(compressed);
                    }
                }
            }
        }
    }
}
