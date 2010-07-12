using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.FileProcessor;
using System.IO;
using Yahoo.Yui.Compressor;

namespace Punchy.Plugin.YahooYuiCompressor
{
    public class YuiProcessor : IFileProcessor
    {
        public void Process(ICollection<FileInfo> workspace)
        {
            foreach (FileInfo fi in workspace)
            {
                string extension = Path.GetExtension(fi.Name).ToLower();
                if (extension == ".css" || extension == ".js")
                {
                    string compressed = null;
                    using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            switch (extension)
                            {
                                case ".css":
                                    compressed = CssCompressor.Compress(reader.ReadToEnd());
                                    break;
                                case ".js":
                                    compressed = JavaScriptCompressor.Compress(reader.ReadToEnd());
                                    break;
                            }
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(fi.FullName, false))
                    {
                        writer.Write(compressed);
                    }
                }
            }
        }
    }
}
