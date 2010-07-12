using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.FileProcessor;
using Punchy.Configuration;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Punchy
{
    public class Processor
    {
        private readonly List<IFileProcessor> processors;
        private readonly List<IBundle> bundles;
        private readonly string outputPath;

        public string GetResourceFor(string bundlefilename)
        {
            throw new NotImplementedException();
        }

        private string ProcessAndSaveBundle(IBundle bundle)
        {
            throw new NotImplementedException();
        }

        private bool TryCache(string bundlefilename, out string filename)
        {
            throw new NotImplementedException();
        }

        public Processor(ICacheProvider cacheProvider)
        {
            PunchyConfigurationSection config = (PunchyConfigurationSection)ConfigurationManager.GetSection("punchy");

            this.processors = new List<IFileProcessor>(config.Processors.Count);
            this.outputPath = config.OutputFolder;
            foreach (ProviderSettings plugin in config.Processors)
            {
                this.processors.Add(CreatePluginInstanceFromTypeString<IFileProcessor>(plugin.Type));
            }

            this.bundles = new List<IBundle>(config.Bundles.Cast<IBundle>());
        }

        /// <summary>
        /// Converts this string into an instance of the specified type if the string is in standard type pattern format.
        /// </summary>
        /// <param name="value">The type string, which consists of the fully qualified type name followed by the assembly name that it is found in.</param>
        private T CreatePluginInstanceFromTypeString<T>(string value)
        {
            Regex re = new Regex(@"^(?<type>(\w+(\.?\w+)+))\s*,\s*(?<assembly>[\w\.]+)(,\s?Version=(?<version>\d+\.\d+\.\d+\.\d+))?(,\s?Culture=(?<culture>\w+))?(,\s?PublicKeyToken=(?<token>\w+))?$", RegexOptions.None);
            Match match = re.Match(value);
            if (match.Success)
            {
                try
                {
                    return (T)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(match.Groups["assembly"].Value, match.Groups["type"].Value);
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidPunchyConfigurationException("Plugin type \"" + value + "\" was not of type \"" + typeof(T).AssemblyQualifiedName + "\".", ex);
                }
            }
            else
            {
                throw new InvalidPunchyConfigurationException("Plugin type string \"" + value + "\" was not in the correct format.");
            }
        }
    }
}
