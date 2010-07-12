using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Punchy.FileProcessor
{
    public interface IFileProcessor
    {
        ICollection<FileInfo> Process(ICollection<FileInfo> fileset, string tempfolder);
    }
}
