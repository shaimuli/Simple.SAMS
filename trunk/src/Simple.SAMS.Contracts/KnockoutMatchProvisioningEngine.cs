using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts
{
    public class KnockoutMatchProvisioningEngine : IMatchProvisioningEngine
    {
        public MatchHeaderInfo[] BuildMatches(CompetitionType competitionType)
        {
            var playersCount = competitionType.PlayersCount;
            var rounds = (int) Math.Log((playersCount), 2);
            var matches = new List<MatchHeaderInfo>(playersCount);
            for (int i = 0; i < playersCount; i++)
            {
                var match = new MatchHeaderInfo();
                match.Position = i;
                match.Round =   rounds - Math.Max(0,(int)Math.Log((playersCount - (i+1)), 2));
                match.Status = MatchStatus.Created;
                matches.Add(match);
            }
            return matches.ToArray();
        }
    }
}
