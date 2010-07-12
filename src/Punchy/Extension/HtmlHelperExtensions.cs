using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Punchy
{
    public static class HtmlHelperExtensions
    {
        public static string PunchyBundleLinkCss(this HtmlHelper html, string bundlefilename)
        {
            string url = ProcessorWrapper.Instance.GetResourceFor(bundlefilename);
            return "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + url + "\"/>";
        }

        public static string PunchyBundleScript(this HtmlHelper html, string bundlefilename)
        {
            string url = ProcessorWrapper.Instance.GetResourceFor(bundlefilename);
            return "<script type=\"text/javascript\" src=\"" + url + "\"></script>";
        }
    }
}
