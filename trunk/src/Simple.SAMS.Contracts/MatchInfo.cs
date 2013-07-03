using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnockoutEngine
{
    public class MatchInfo
    {
        public int Index { get; set; }
        public string Player1Code { get; set; }
        public string Player2Code { get; set; }
        public string SectionCode { get; set; }
        public override string ToString()
        {
            return string.Format("{0}: {1} <-> {2}", Index + 1, Player1Code, Player2Code);
        }
    }
}
