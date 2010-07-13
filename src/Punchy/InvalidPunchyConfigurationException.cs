using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy
{
    [Serializable]
    public class InvalidPunchyConfigurationException : ApplicationException
    {
        public InvalidPunchyConfigurationException()
            : base("Punchy configuration is missing or invalid.")
        { }

        public InvalidPunchyConfigurationException(string message)
            : base(message)
        { }

        public InvalidPunchyConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
