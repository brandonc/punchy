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
        public static MvcHtmlString PunchyBundleLinkCss(this HtmlHelper html, string bundlefilename, object htmlAttributes)
        {
            if (ProcessorWrapper.Instance.DebugMode)
            {
                var bundle = ProcessorWrapper.Instance.GetBundleFor(bundlefilename);

                var result = new StringBuilder(512);
                foreach (IFile file in bundle.FileList)
                {
                    result.AppendLine(BuildLinkTag("/" + VirtualPathUtility.MakeRelative(html.ViewContext.HttpContext.Request.ApplicationPath, file.VirtualPath), htmlAttributes));
                }

                return MvcHtmlString.Create(result.ToString());
            }
            else
            {
                return MvcHtmlString.Create(BuildLinkTag(ProcessorWrapper.Instance.GetResourceFor(bundlefilename), htmlAttributes));
            }
        }

        private static string BuildLinkTag(string href, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("link");
            builder.Attributes.Add("href", href);
            builder.Attributes.Add("type", "text/css");

            if (htmlAttributes != null)
                builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes), false);

            if (!builder.Attributes.ContainsKey("rel"))
                builder.Attributes.Add("rel", "stylesheet");

            return builder.ToString(TagRenderMode.SelfClosing);
        }

        public static MvcHtmlString PunchyBundleLinkCss(this HtmlHelper html, string bundlefilename)
        {
            return PunchyBundleLinkCss(html, bundlefilename, null);
        }

        public static MvcHtmlString PunchyBundleScript(this HtmlHelper html, string bundlefilename, object htmlAttributes)
        {
            if (ProcessorWrapper.Instance.DebugMode)
            {
                var bundle = ProcessorWrapper.Instance.GetBundleFor(bundlefilename);

                var result = new StringBuilder(512);
                foreach (IFile file in bundle.FileList)
                {
                    result.AppendLine(BuildScriptTag("/" + VirtualPathUtility.MakeRelative(html.ViewContext.HttpContext.Request.ApplicationPath, file.VirtualPath), htmlAttributes));
                }

                return MvcHtmlString.Create(result.ToString());
            }
            else
            {
                return MvcHtmlString.Create(BuildScriptTag(ProcessorWrapper.Instance.GetResourceFor(bundlefilename), htmlAttributes));
            }
        }

        public static MvcHtmlString PunchyBundleScript(this HtmlHelper html, string bundlefilename)
        {
            return PunchyBundleScript(html, bundlefilename, null);
        }

        private static string BuildScriptTag(string src, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("script");
            builder.Attributes.Add("src", src);
            builder.Attributes.Add("type", "text/javascript");

            if (htmlAttributes != null)
                builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes), true);

            return builder.ToString(TagRenderMode.Normal);
        }
    }
}
