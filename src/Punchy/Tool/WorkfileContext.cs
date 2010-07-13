using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Punchy.Tool
{
    public class WorkfileContext
    {
        public FileInfo Workfile { get; set;  }
        public FileInfo OriginalSource { get; set; }
    }
}
