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
            var matches = new List<MatchHeaderInfo>();

            var finalSectionMatches = CreateSectionMatches(competitionType.PlayersCount, CompetitionSection.Final);
            var qualifyingSectionMatches = CreateSectionMatches(competitionType.QualifyingPlayersCount, CompetitionSection.Qualifying);

            matches.AddRange(qualifyingSectionMatches);
            matches.AddRange(finalSectionMatches);

            return matches.ToArray();
        }

        private static IEnumerable<MatchHeaderInfo> CreateSectionMatches(int playersCount, CompetitionSection section)
        {
            var rounds = (int) Math.Log((playersCount), 2);
            var matches = new List<MatchHeaderInfo>(playersCount);
            for (int i = 0; i < playersCount; i++)
            {
                var match = new MatchHeaderInfo();
                match.Section = section;
                match.Position = i;
                match.Round = rounds - Math.Max(0, (int) Math.Log((playersCount - (i + 1)), 2));
                match.Status = MatchStatus.Created;
                matches.Add(match);
            }
            return matches;
        }
    }
}
