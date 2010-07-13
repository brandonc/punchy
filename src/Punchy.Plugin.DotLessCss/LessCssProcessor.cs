using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.Tool;
using System.IO;
using dotless.Core;
using dotless.Core.configuration;

namespace Punchy.Plugin.DotLessCss
{
    public class LessCssProcessor : ITool
    {
        public void Process(ToolContext context)
        {
            foreach (WorkfileContext workContext in context.Workfiles)
            {
                Directory.SetCurrentDirectory(workContext.OriginalSource.DirectoryName);

                if (Path.GetExtension(workContext.Workfile.Name).ToLower() == ".less")
                {
                    string compressed = null;
                    using (var reader = new StreamReader(workContext.Workfile.FullName, true))
                    {
                        var engine = new EngineFactory(new DotlessConfiguration()
                        {
                            CacheEnabled = false,
                            MinifyOutput = false,
                            Web = false,
                        }).GetEngine();

                        compressed = engine.TransformToCss(reader.ReadToEnd(), null);
                        IEnumerable<string> l = engine.GetImports();
                    }

                    string newFileName = Path.Combine(workContext.Workfile.DirectoryName, Path.GetFileNameWithoutExtension(workContext.Workfile.Name)) + ".css";
                    workContext.Workfile.MoveTo(newFileName);

                    using (var writer = new StreamWriter(newFileName, false))
                    {
                        writer.Write(compressed);
                    }
                }
            }
        }
    }
}
