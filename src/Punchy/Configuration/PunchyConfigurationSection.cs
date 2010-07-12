using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Punchy.FileProcessor;

namespace Punchy.Configuration
{
    public class PunchyConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("outputfolder", DefaultValue = "~/static/cache", IsRequired = false)]
        public string OutputFolder
        {
            get
            {
                return PathMapper.MapPath((string)this["outputfolder"]);
            }
        }

        [ConfigurationProperty("bundles")]
        [ConfigurationCollection(typeof(BundleElementCollection), AddItemName = "bundle", ClearItemsName = "clearbundles", RemoveItemName = "removebundle")]
        public BundleElementCollection Bundles
        {
            get
            {
                return (BundleElementCollection)this["bundles"];
            }
        }

        [ConfigurationProperty("processors")]
        [ConfigurationCollection(typeof(ProviderSettingsCollection))]
        public ProviderSettingsCollection Processors
        {
            get
            {
                return (ProviderSettingsCollection)this["processors"];
            }
        }
    }
}
