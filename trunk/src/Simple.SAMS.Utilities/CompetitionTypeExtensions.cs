using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Utilities
{
    public static class CompetitionTypeExtensions
    {
        public static int RankPlayer(this CompetitionType competitionType, Player player)
        {
            var rank = player.NationalRank;
            if (competitionType.Ranking == Ranking.YouthInternational)
            {
                rank = player.YouthInternationalRank;
            }
            else if (competitionType.Ranking == Ranking.EuropeInternational)
            {
                rank = player.EuropeInternationalRank;
            }
            return rank.GetValueOrDefault();
        }
    }
}
