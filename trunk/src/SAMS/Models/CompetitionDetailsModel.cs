using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace SAMS.Models
{
    public class CompetitionDetailsModel
    {
        public bool PlayingStarted { get; set; }
        public bool CanAddToQualifying { get; set; }
        public bool CanAddToFinal { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReferenceId { get; set; }
        public string MainRefereeName { get; set; }
        public string MainRefereePhone { get; set; }
        public string SitePhone { get; set; }
        public string Site { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string LastModified { get; set; }
        public CompetitionStatus Status { get; set; }
        public CompetitionType Type { get; set; }

        public CompetitionMatchViewModel[] Matches { get; set; }
        public CompetitionPlayer[] Players { get; set; }
    }
}