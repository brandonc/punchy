using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punchy.Tool;
using System.IO;
using System.Web;
using System.Net;

namespace Punchy.Plugin.GoogleClosure
{
    public class ClosureProcessor : ITool
    {
        private const string postFormat = "js_code={0}&compilation_level={1}&output_format={2}&output_info={3}";
        private const long maxLength = 200000;

        public void Process(ToolContext context)
        {
            long totalLength = 0;
            foreach (WorkfileContext workContext in context.Workfiles)
                totalLength += workContext.Workfile.Length;

            if (totalLength > maxLength)
                throw new InvalidOperationException("Google Closure plugin only supports bundles up to 200K. That's because this plugin is hacky.");

            foreach (WorkfileContext workContext in context.Workfiles)
            {
                var request = WebRequest.Create("http://closure-compiler.appspot.com/compile");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                using (var fileReader = new StreamReader(workContext.Workfile.FullName))
                {
                    var data = string.Format(
                        postFormat,
                        HttpUtility.UrlEncode(fileReader.ReadToEnd()),
                        "WHITESPACE_ONLY",
                        "text",
                        "compiled_code"
                    );
                    
                    using (var dataStream = new StreamWriter(request.GetRequestStream()))
                    {
                        dataStream.Write(data);
                    }
                }

                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    var streamReader = new StreamReader(responseStream);

                    using (var writer = new StreamWriter(workContext.Workfile.FullName, false))
                    {
                        writer.Write(streamReader.ReadToEnd());
                    }
                }
            }
        }
    }
}
