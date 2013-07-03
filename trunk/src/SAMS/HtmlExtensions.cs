using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Simple;
using Simple.SAMS.CompetitionEngine;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS
{
    public static class HtmlExtensions
    {
        public static IEnumerable<SelectListItem> TimeHours(this HtmlHelper htmlHelper, DateTime? time = default(DateTime?))
        {
            var items = Enumerable.Range(0, 24).Select(i => new SelectListItem() { Value = i.ToString(), Text = i.ToString().PadLeft(2, '0') }).ToArray();
            if (time.HasValue)
            {
                var selected = items.FirstOrDefault(i => int.Parse(i.Value) >= time.Value.ToLocalTime().Hour);
                if (selected.IsNotNull())
                {
                    selected.Selected = true;
                }
            }
            return new[] {new SelectListItem() }.Concat(items);
        }
        public static IEnumerable<SelectListItem> TimeMinutes(this HtmlHelper htmlHelper, DateTime? time = default(DateTime?))
        {
            var items = Enumerable.Range(0, 59).Where(i => i % 15 == 0).Select(
                    i => new SelectListItem() { Value = i.ToString(), Text = i.ToString().PadLeft(2, '0') }).ToArray();
            if (time.HasValue)
            {
                var selected = items.FirstOrDefault(i => int.Parse(i.Value) >= time.Value.ToLocalTime().Minute);
                if (selected.IsNotNull())
                {
                    selected.Selected = true;
                }
            }
            return new[] {new SelectListItem() }.Concat(items);
        }

        public static IHtmlString MatchStatuses(this HtmlHelper htmlHelper, string name="",MatchStatus? selected=default(MatchStatus))
        {
            return htmlHelper.DropDownList(name, Enumerable.Range(-1, 5).Cast<MatchStatus>().Select(s => new SelectListItem()
                                                                                         {
                                                                                             Value = s.ToString(),
                                                                                             Text = htmlHelper.ResourceText("MatchStatus." + s),
                                                                                             Selected = selected.HasValue && selected.Value == s
                                                                                         }));
        }
        public static IHtmlString SectionSelect(this HtmlHelper htmlHelper, string name = "Section", int selected = 0)
        {
            return htmlHelper.DropDownList(name, Enumerable.Range(0,5).Select(s=> 
                                                         new SelectListItem()
                                                             {
                                                                 Value = s.ToString(),
                                                                 Text = htmlHelper.SectionName(s),
                                                                 Selected = s == selected
                                                             }));
        }
        public static IHtmlString RoundSelect(this HtmlHelper htmlHelper, string name = "Round", int selected = 0)
        {
            var items = Enumerable.Range(0, 7).Select(r =>
                                          new SelectListItem()
                                              {
                                                  Value = r.ToString(),
                                                  Text = htmlHelper.RoundName(r),
                                                  Selected = r == selected
                                              }).ToList();
            items.Insert(2, new SelectListItem()
                                {
                                    Value = "1.5",
                                    Text = htmlHelper.RoundName(1,2),
                                    Selected = false
                                });
            items[1].Text = htmlHelper.RoundName(1, 1);
            return htmlHelper.DropDownList(name, items);
        }

        public static string SectionName(this HtmlHelper helper, int section)
        {
            var key = "AllSections";
            if (section > 0)
            {
                key = "CompetitionSection." + ((CompetitionSection) section).ToString();
            }
            return helper.ResourceText(key);
        }


        public static string RoundName(this HtmlHelper helper, int round, int part=0)
        {
            var result = helper.ResourceText("Round") + " " + round;
            if (part > 0)
            {
                result += " - " + part;
            }
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
                result = helper.ResourceText("AllRounds");
            }

            return result;
        }
        public static int RoundMatchesCount(this HtmlHelper helper,  int count)
        {
            return PlayersCountCalculator.CalculatePlayersCount(count);
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