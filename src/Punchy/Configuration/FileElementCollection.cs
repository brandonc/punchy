using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy.Configuration
{
    public class FileElementCollection : ConfigurationElementCollection
    {
        public new FileElement this[string path]
        {
            get
            {
                return (FileElement)BaseGet(path);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileElement)element).Path;
        }
    }
}
