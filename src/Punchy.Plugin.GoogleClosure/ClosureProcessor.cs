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

        public void Process(ICollection<System.IO.FileInfo> workspace)
        {
            foreach (FileInfo fi in workspace)
            {
                using (var fileReader = new StreamReader(fi.FullName))
                {
                    var data = string.Format(
                        postFormat,
                        HttpUtility.UrlEncode(fileReader.ReadToEnd()),
                        "WHITESPACE_ONLY",
                        "text",
                        "compiled_code"
                    );

                    var request = WebRequest.Create("http://closure-compiler.appspot.com/compile");
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    using (var dataStream = new StreamWriter(request.GetRequestStream()))
                    {
                        dataStream.Write(data);
                    }

                    var response = request.GetResponse();
                    using (var responseStream = response.GetResponseStream())
                    {
                        var streamReader = new StreamReader(responseStream);

                        using (var writer = new StreamWriter(fi.FullName, false))
                        {
                            writer.Write(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }
}
