using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Models
{
    public class MatchResultUpdateModel
    {
        public int Version { get; set; }
        public int Id { get; set; }
        public SetScore[] SetScores { get; set; }
        public bool Player1Won { get; set; }
        public int? StartTimeHours { get; set; }
        public int? StartTimeMinutes { get; set; }
        public string Date { get; set; }
        public StartTimeType? StartTimeType { get; set; }
        public MatchWinner? Winner { get; set; }
        public MatchResult? Result { get; set; }

    }
}