using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Punchy.Tool
{
    public class ToolContext
    {
        public ICollection<WorkfileContext> Workfiles { get; set; }
        public FileInfo OutputFile { get; set; }
    }
}
