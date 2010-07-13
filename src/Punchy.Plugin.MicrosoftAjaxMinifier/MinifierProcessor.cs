using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.Tool;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace Punchy.Plugin.MicrosoftAjaxMinifier
{
    public class MinifierProcessor : ITool
    {
        public void Process(ToolContext context)
        {
            var minifier = new Minifier();

            foreach (WorkfileContext workContext in context.Workfiles)
            {
                string extension = Path.GetExtension(workContext.Workfile.Name).ToLower();
                if (extension == ".css" || extension == ".js")
                {
                    string compressed = null;
                    using (var stream = new FileStream(workContext.Workfile.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            switch (extension)
                            {
                                case ".css":
                                    compressed = minifier.MinifyStyleSheet(reader.ReadToEnd());
                                    break;
                                case ".js":
                                    compressed = minifier.MinifyJavaScript(reader.ReadToEnd());
                                    break;
                            }
                        }
                    }

                    using (var writer = new StreamWriter(workContext.Workfile.FullName, false))
                    {
                        writer.Write(compressed);
                    }
                }
            }
        }
    }
}
