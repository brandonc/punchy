using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punchy.Configuration
{
    public class BundleElement : ConfigurationElement, IBundle
    {
        private string mimeType = null;

        [ConfigurationProperty("outfile", IsRequired = true, IsKey = true)]
        public string OutFile
        {
            get
            {
                return (string)this["outfile"];
            }
            set
            {
                this["outfile"] = value;
            }
        }

        [ConfigurationProperty("files", IsRequired = true)]
        [ConfigurationCollection(typeof(FileElementCollection))]
        public FileElementCollection Files
        {
            get
            {
                return (FileElementCollection)this["files"];
            }
        }

        public string MimeType
        {
            get {
                if(this.mimeType == null)
                    this.mimeType = GetMimeType();

                return this.mimeType;
            }
        }

        private string GetMimeType()
        {
            string result = null;
            foreach (FileElement file in this.Files)
            {
                switch (Path.GetExtension(file.Path))
                {
                    case ".js":
                        if (result != null && result != "text/javascript")
                            ThrowSetCombinationException();
                        result = "text/javascript";
                        break;
                    case ".css":
                    case ".less":
                        if (result != null && result != "text/css")
                            ThrowSetCombinationException();
                        result = "text/css";
                        break;
                    default:
                        throw new InvalidPunchyConfigurationException("Unknown file extension \"" + Path.GetExtension(file.Path) + "\" in fileset \"" + this.OutFile + "\".");
                }
            }

            return result;
        }

        private void ThrowSetCombinationException()
        {
            throw new InvalidPunchyConfigurationException("Fileset \"" + this.OutFile + "\" has an invalid combination of file types.");
        }

        public ICollection<IFile> FileList
        {
            get {
                return new List<IFile>(this.Files.Cast<IFile>());
            }
        }

        public string Filename
        {
            get { return this.OutFile; }
        }
    }
}
