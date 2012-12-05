using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.CompetitionEngine
{
    public class QualifyingPositionParameters
    {
        public int PlayersCount { get; set; }
        public int QualifyingCount { get; set; }
        public Player[] Players { get; set; }
    }
}
