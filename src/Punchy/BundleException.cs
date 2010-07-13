using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy
{
    [Serializable]
    public class BundleException : ApplicationException
    {
        public static BundleException FromFilename(string filename)
        {
            return new BundleException("The file \"" + filename + "\" could not be accessed or does not exist.");
        }

        public BundleException(string message)
            : base(message)
        { }

        public BundleException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
