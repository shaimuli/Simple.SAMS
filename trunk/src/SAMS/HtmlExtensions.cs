﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS
{
    public static class HtmlExtensions
    {
        public static IHtmlString MatchStatuses(this HtmlHelper htmlHelper, string name="",MatchStatus? selected=default(MatchStatus))
        {
            return htmlHelper.DropDownList(name, Enumerable.Range(-1, 5).Cast<MatchStatus>().Select(s => new SelectListItem()
                                                                                         {
                                                                                             Value = s.ToString(),
                                                                                             Text = htmlHelper.ResourceText("MatchStatus." + s),
                                                                                             Selected = selected.HasValue && selected.Value == s
                                                                                         }));
        }
        public static IHtmlString RoundSelect(this HtmlHelper htmlHelper, string name = "Round", int selected = 0)
        {
            return htmlHelper.DropDownList(name, Enumerable.Range(0,7).Select(r=> 
                                                         new SelectListItem()
                                                             {
                                                                 Value = r.ToString(),
                                                                 Text = htmlHelper.RoundName(r),
                                                                 Selected = r == selected
                                                             }));
        }

        public static string RoundName(this HtmlHelper helper, int round)
        {
            var result = helper.ResourceText("Round") + " " + round;

            if (round == 3)
            {
                result = helper.ResourceText("QuarterFinal");
            }
            else if (round == 4)
            {
                result = helper.ResourceText("SemiFinal");
            }
            else if (round == 5)
            {
                result = helper.ResourceText("Final");
            }
            else if (round == 6)
            {
                result = helper.ResourceText("ThirdPlace");
            } else if (round <= 0)
            {
                result = string.Empty;
            }

            return result;
        }
        public static int RoundMatchesCount(this HtmlHelper helper,  int count)
        {
            if (count == 24)
            {
                count = 32;
            } else if (count == 48)
            {
                count = 64;
            }

            return count;
        }

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