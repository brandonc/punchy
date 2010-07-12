using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Punchy
{
    interface IBundle
    {
        string Filename { get; }
        ICollection<IFile> FileList { get; }
        string MimeType { get; }
        long Revision { get; set; }
    }
}
