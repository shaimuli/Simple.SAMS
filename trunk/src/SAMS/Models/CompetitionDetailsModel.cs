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
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReferenceId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string LastModified { get; set; }
        public CompetitionStatus Status { get; set; }
        public EntityReference Type { get; set; }
        public CompetitionMatchViewModel[] Matches { get; set; }
        public CompetitionPlayer[] Players { get; set; }
    }
}