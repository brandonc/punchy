using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Punchy.Configuration
{
    public class ToolchainElementCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("for")]
        public string ForMimeType
        {
            get
            {
                return (string)this["for"];
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ToolchainElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ToolchainElement)element).Type;
        }
    }
}
