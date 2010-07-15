using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace Punchy
{
    public static class HtmlHelperExtensions
    {
        public static string PunchyBundleLinkCss(this HtmlHelper html, string bundlefilename, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("link");
            builder.Attributes.Add("href", ProcessorWrapper.Instance.GetResourceFor(bundlefilename));
            builder.Attributes.Add("type", "text/css");

            if(htmlAttributes != null)
                builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes), false);

            if(!builder.Attributes.ContainsKey("rel"))
                builder.Attributes.Add("rel", "stylesheet");

            return builder.ToString(TagRenderMode.SelfClosing);
        }

        public static string PunchyBundleLinkCss(this HtmlHelper html, string bundlefilename)
        {
            return PunchyBundleLinkCss(html, bundlefilename, null);
        }

        public static string PunchyBundleScript(this HtmlHelper html, string bundlefilename, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("script");
            builder.Attributes.Add("src", ProcessorWrapper.Instance.GetResourceFor(bundlefilename));
            builder.Attributes.Add("type", "text/javascript");

            if (htmlAttributes != null)
                builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes), true);

            return builder.ToString(TagRenderMode.Normal);
        }

        public static string PunchyBundleScript(this HtmlHelper html, string bundlefilename)
        {
            return PunchyBundleScript(html, bundlefilename, null);
        }
    }
}
