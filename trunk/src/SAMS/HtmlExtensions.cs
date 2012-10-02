using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAMS
{
    public static class HtmlExtensions
    {
        public static IHtmlString Resource(this HtmlHelper helper, string key)
        {
            return new HtmlString(helper.ResourceText(key));
        }

        public static string ResourceText(this HtmlHelper helper, string key)
        {
            return Resources.Texts.ResourceManager.GetString(key) ?? key;
        }

    }
}