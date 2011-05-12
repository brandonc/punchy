using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy
{
    public interface IFile
    {
        string Key { get; }
        string PhysicalPath { get; }
        string VirtualPath { get; }
    }
}
