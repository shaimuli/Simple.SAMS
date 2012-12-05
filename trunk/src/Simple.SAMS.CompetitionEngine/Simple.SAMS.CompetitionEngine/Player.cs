using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.CompetitionEngine
{
    public class Player
    {
        public int Rank { get; set; }
        public int Id { get; set; }
        public override string ToString()
        {
            return Id + ": " + Rank;
        }
    }
}
