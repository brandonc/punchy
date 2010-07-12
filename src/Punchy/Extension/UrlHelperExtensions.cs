using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Punchy
{
    public static class UrlHelperExtensions
    {
        public static string PunchyBundleUrl(this UrlHelper url, string bundlefilename)
        {
            return url.Content(ProcessorWrapper.Instance.GetResourceFor(bundlefilename));
        }
    }
}
