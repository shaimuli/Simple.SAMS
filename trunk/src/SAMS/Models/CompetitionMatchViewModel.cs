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
        public CompetitionSection Section { get; set; }
        public int? Position { get; set; }

        public int Round { get; set; }

        public DateTime? StartTime { get; set; }
        public MatchPlayerViewModel Player1 { get; set; }
        public MatchPlayerViewModel Player2 { get; set; }
        public MatchPlayerViewModel Player3 { get; set; }
        public MatchPlayerViewModel Player4 { get; set; }

    }
}