using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Punchy.Tool
{
    class CombinerTool : ITool
    {
        public void Process(ToolContext context)
        {
            // Combine and save workfiles into target output file
            using (var writer = new StreamWriter(context.OutputFile.FullName, false, Encoding.UTF8, 1024))
            {
                foreach (WorkfileContext workfile in context.Workfiles)
                {
                    using (var reader = new StreamReader(workfile.Workfile.FullName, true))
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
