using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Models
{
    public class CompetitionMatchViewModel
    {
        public int Id { get; set; }

        public MatchStatus Status { get; set; }

        public int? Position { get; set; }

        public int Round { get; set; }

        public DateTime? StartTime { get; set; }
        public MatchPlayerViewModel LeftPlayer1 { get; set; }
        public MatchPlayerViewModel LeftPlayer2 { get; set; }

        public MatchPlayerViewModel RightPlayer1 { get; set; }
        public MatchPlayerViewModel RightPlayer2 { get; set; }

    }
}