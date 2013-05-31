using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    public class UpdatePlayerPointsInfo
    {
        public int PlayerId { get; set; }
        public int? Points { get; set; }
        public int? Position { get; set; }
    }
}
