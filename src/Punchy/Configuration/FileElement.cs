using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Punchy.Configuration
{
    public class FileElement : ConfigurationElement, IFile
    {
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
        }

        public string PhysicalPath
        {
            get {
                return PathMapper.MapPath(this.Path);
            }
        }

        public string Key
        {
            get
            {
                return this.Path;
            }
        }

        public string VirtualPath
        {
            get
            {
                return this.Path;
            }
        }
    }
}
