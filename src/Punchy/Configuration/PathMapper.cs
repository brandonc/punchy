using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Punchy.Configuration
{
    class PathMapper
    {
        public static string MapPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(path);
            }

            return Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                Path.GetDirectoryName(path.Replace("~/", "")),
                Path.GetFileName(path)
            );
        }
    }
}
