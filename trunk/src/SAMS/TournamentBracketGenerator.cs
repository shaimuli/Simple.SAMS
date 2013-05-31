using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Simple;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS
{
    public class TournamentBracketGenerator
    {
        private void WriteRound(HtmlTextWriter htmlWriter, int round, int minRound, Dictionary<int, Queue<MatchHeaderInfo>> map)
        {
            if (round == minRound)
            {
                return;
            }
            var match = default(MatchHeaderInfo);
            if (map.ContainsKey(round))
            {
                var queue = map[round];
                match = queue.Dequeue();


            }

            RenderContainer(htmlWriter, round, minRound, map, match, 0);
            RenderContainer(htmlWriter, round, minRound, map, match, 1);


        }

        private void RenderContainer(HtmlTextWriter htmlWriter, int round, int minRound, Dictionary<int, Queue<MatchHeaderInfo>> map, MatchHeaderInfo match, int player)
        {
            var side = player == 0 ? "top" : "bottom";
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "round" + round + "-" + side + "wrap");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
            if (match.IsNotNull())
            {
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "round" + round + "-" + side);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                RenderPlayer(htmlWriter, match, side == "top" ? match.Player1 : match.Player2);
                RenderScores(htmlWriter, match, player);
                htmlWriter.RenderEndTag();
            }

            WriteRound(htmlWriter, round - 1, minRound, map);

            htmlWriter.RenderEndTag();
        }

        private static void RenderScores(HtmlTextWriter htmlWriter, MatchHeaderInfo match, int player)
        {
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "scores");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
            if (match.SetScores.IsNullOrEmpty() && (int)match.Status < (int)MatchStatus.Completed)
            {
                if (player == 0)
                {

                    htmlWriter.Write(match.Date);
                }
                else if (match.StartTime.HasValue)
                {
                    htmlWriter.Write(match.StartTime.Value.ToString("HH:mm"));
                }
            }
            else
            {
                foreach (var setScore in match.SetScores)
                {
                    
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                    var points = (player == 0 ? setScore.Player1Points : setScore.Player2Points);
                    htmlWriter.Write(points == 0 ? " " : points.ToString());
                    htmlWriter.RenderEndTag();
                }
            }
            htmlWriter.RenderEndTag();
        }

        private static void RenderPlayer(HtmlTextWriter htmlWriter, MatchHeaderInfo match, MatchPlayer player)
        {
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "player");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
            if (player != null)
            {
                htmlWriter.Write(player.LocalFirstName);
                if (player.LocalLastName.NotNullOrEmpty())
                {
                    htmlWriter.Write(" , ");
                    htmlWriter.Write(player.LocalLastName);
                }
            }
            else
            {
                htmlWriter.Write(match.Status == MatchStatus.Completed ? "BYE" : "&nbsp;");
            }
            htmlWriter.RenderEndTag();
        }

        public string Generate(CompetitionDetails competition, CompetitionSection section)
        {
            var result = string.Empty;
            var map = new Dictionary<int, Queue<MatchHeaderInfo>>();

            var sectionMatches = competition.Matches.Where(m => m.Section == section);
            foreach (var match in sectionMatches)
            {
                var matches = default(Queue<MatchHeaderInfo>);
                if (map.TryGetValue(match.Round, out matches))
                {
                    matches.Enqueue(match);
                }
                else
                {
                    matches = new Queue<MatchHeaderInfo>();
                    matches.Enqueue(match);
                    map[match.Round] = matches;
                }
            }
            var rounds = map.Keys.Max();
            using (var stringWriter = new StringWriter())
            using (var htmlWriter = new HtmlTextWriter(stringWriter))
            {
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Html);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Head);

                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, VirtualPathUtility.ToAbsolute("~/Static/Css/bracket.css"));
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Link);
                htmlWriter.RenderEndTag();

                htmlWriter.RenderEndTag();
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Body);

                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "header");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                htmlWriter.Write(competition.Name);
                htmlWriter.RenderEndTag();

                if (competition.MainRefereeName.NotNullOrEmpty())
                {
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "referee");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                    htmlWriter.Write(competition.MainRefereeName);
                    if (competition.MainRefereePhone.NotNullOrEmpty())
                    {
                        htmlWriter.Write(", ");
                        htmlWriter.Write(competition.MainRefereePhone);
                    }
                    htmlWriter.RenderEndTag();
                }

                if (competition.Site.NotNullOrEmpty())
                {
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "site");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                    htmlWriter.Write(competition.Site);
                    if (competition.SitePhone.NotNullOrEmpty())
                    {
                        htmlWriter.Write(", ");
                        htmlWriter.Write(competition.SitePhone);
                    }
                    htmlWriter.RenderEndTag();
                }

                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "rounds clearfix");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                foreach (var round in sectionMatches.Select(m => m.Round).Distinct())
                {
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "round");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                    htmlWriter.Write(HtmlExtensions.RoundName(null, round));
                    htmlWriter.RenderEndTag();
                }
                htmlWriter.RenderEndTag();
                var tournamentMatches = sectionMatches.Count();
                if (tournamentMatches > 8)
                {
                    tournamentMatches = ((tournamentMatches/16) + 1)*16;
                }
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "tournament" + tournamentMatches + "-wrap");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "round6-top winner6");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                htmlWriter.RenderEndTag();

                WriteRound(htmlWriter, 6, 0, map);

                htmlWriter.RenderEndTag();
                htmlWriter.RenderEndTag();
                htmlWriter.RenderEndTag();
                htmlWriter.Flush();
                result = stringWriter.ToString();

            }

            return result;
        }
    }
}