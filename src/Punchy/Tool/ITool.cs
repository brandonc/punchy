using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Punchy.Tool
{
    public interface ITool
    {
        void Process(ICollection<FileInfo> workspace);
    }
}
