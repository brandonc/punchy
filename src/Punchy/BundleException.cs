using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy
{
    public class BundleException : ApplicationException
    {
        public BundleException(string filename)
            : base("The file \"" + filename + "\" could not be accessed or does not exist.")
        { }
    }
}
