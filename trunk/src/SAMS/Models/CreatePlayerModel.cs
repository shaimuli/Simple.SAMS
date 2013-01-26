using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace SAMS.Models
{
    public class CreatePlayerModel
    {
        public CompetitionHeaderInfo Competition { get; set; }
        public Player ReplacedPlayer { get; set; }
        public Player Player { get; set; }
        public CompetitionPlayerSource Source { get; set; }
    }
}