using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Punchy.Tool
{
    class CombinerTool : ITool
    {
        public void Process(ICollection<FileInfo> workspace)
        {
            // Combine and save into bundle file
            string outputFile = "tmp.txt";// Path.Combine(this.outputPath, target.Filename);
            using (StreamWriter writer = new StreamWriter(outputFile, false, Encoding.UTF8, 1024))
            {
                foreach (FileInfo fi in workspace)
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
        }
    }
}
