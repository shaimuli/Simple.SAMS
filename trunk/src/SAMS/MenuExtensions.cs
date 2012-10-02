using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SAMS.Models;
using Simple;

namespace SAMS
{
    public static class MenuExtensions
    {

        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, MenuItem menuItem)
        {
            Requires.ArgumentNotNull(menuItem, "menuItem");

            return htmlHelper.MenuItem(menuItem.Text, menuItem.Action, menuItem.Controller);
        }

        public static MvcHtmlString MenuItem(
            this HtmlHelper htmlHelper,
            string text,
            string action,
            string controller,
            string group =null
        )
        {
            var li = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            var groupTitle = htmlHelper.ViewContext.Controller.ViewBag.GroupTitle;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase) ||
                ((group != null) && (groupTitle == group)))
            {
                li.AddCssClass("active");
            }
            li.InnerHtml = htmlHelper.ActionLink(text, action, controller).ToHtmlString();
            return MvcHtmlString.Create(li.ToString());
        }        
        
        
    }
}