using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Punchy.Configuration
{
    
    public class BundleElementCollection : ConfigurationElementCollection
    {
        public new BundleElement this[string key]
        {
            get
            {
                return (BundleElement)BaseGet(key);
            }
        }

        public BundleElement this[int index]
        {
            get
            {
                return (BundleElement)BaseGet(index);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new BundleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BundleElement)element).OutFile;
        }
    }
}
