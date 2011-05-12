using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Punchy.Tool;

namespace Punchy.Configuration
{
    public class PunchyConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("outputpath", DefaultValue = "~/static/cache", IsRequired = false)]
        public string OutputPath
        {
            get
            {
                return (string)this["outputpath"];
            }
        }

        [ConfigurationProperty("debugmode", DefaultValue = false, IsRequired = false)]
        public bool DebugMode
        {
            get
            {
                return (bool)this["debugmode"];
            }
        }

        public string OutputPhysicalPath
        {
            get
            {
                return PathMapper.MapPath((string)this["outputpath"]);
            }
        }

        [ConfigurationProperty("bundles", IsRequired = true)]
        [ConfigurationCollection(typeof(BundleElementCollection), AddItemName = "bundle", ClearItemsName = "clearbundles", RemoveItemName = "removebundle")]
        public BundleElementCollection Bundles
        {
            get
            {
                return (BundleElementCollection)this["bundles"];
            }
        }

        [ConfigurationProperty("toolchains", IsRequired = true)]
        [ConfigurationCollection(typeof(ToolchainElement), AddItemName = "toolchain", ClearItemsName = "cleartoolchains", RemoveItemName = "removetoolchain")]
        public ToolchainElementCollection Toolchains
        {
            get
            {
                return (ToolchainElementCollection)this["toolchains"];
            }
        }
    }
}
