using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Punchy.Configuration
{
    public class ToolchainElement : ConfigurationElementCollection
    {
        [ConfigurationProperty("mimetype", IsRequired = false)]
        public string ForMimeType
        {
            get
            {
                return (string)this["mimetype"];
            }
        }

        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ToolElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ToolElement)element).Type;
        }
    }
}
