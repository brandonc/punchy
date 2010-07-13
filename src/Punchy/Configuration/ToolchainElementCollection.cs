using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Punchy.Configuration
{
    public class ToolchainElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ToolchainElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ToolchainElement)element).Name;
        }
    }
}
