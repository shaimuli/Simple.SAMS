using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Models
{
    public class CompetitionMatchViewModel
    {
        public int Id { get; set; }

        public MatchStatus Status { get; set; }
        public CompetitionSection Section { get; set; }
        public int? Position { get; set; }

        public int Round { get; set; }

        public DateTime? StartTime { get; set; }
        public MatchPlayerViewModel Player1 { get; set; }
        public MatchPlayerViewModel Player2 { get; set; }
        public MatchPlayerViewModel Player3 { get; set; }
        public MatchPlayerViewModel Player4 { get; set; }
        public SetScore[] SetScores { get; set; }

        public string Score(int player, int setNumber)
        {
            var result = string.Empty;
            if (SetScores.NotNullOrEmpty())
            {
                var setScore = SetScores.FirstOrDefault(s => s.Number == setNumber);
                if (setScore.IsNotNull())
                {
                    switch (player)
                    {
                        case 1:
                            result = setScore.Player1Points.ToString();
                            break;
                        case 2:
                            result = setScore.Player2Points.ToString();
                            break;
                        default:
                            result = setScore.BreakPoints.ToString();
                            break;
                    }
                }
            }

            return result;
        }
    }
}