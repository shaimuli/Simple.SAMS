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
            var qualifyingSectionMatches = CreateSectionMatches(competitionType.QualifyingPlayersCount, CompetitionSection.Qualifying, competitionType.QualifyingToFinalPlayersCount);

            matches.AddRange(qualifyingSectionMatches);
            matches.AddRange(finalSectionMatches);

            return matches.ToArray();
        }

        private IEnumerable<MatchHeaderInfo> CreateSectionMatches(int playersCount, CompetitionSection section, int? qualifyingToNextSection = default(int?))
        {
            var actualPlayersCount = playersCount - qualifyingToNextSection.GetValueOrDefault();
            var rounds = (int) Math.Log((playersCount), 2);
            var actualRounds = rounds - (qualifyingToNextSection.GetValueOrDefault() > 0 ? (int)Math.Log(qualifyingToNextSection.GetValueOrDefault(), 2) : 0);
            var matches = new List<MatchHeaderInfo>(playersCount);

            for (int i = 0; i < actualPlayersCount; i++)
            {
                var match = new MatchHeaderInfo();
                match.Section = section;
                match.Position = i;
                match.Round = rounds - Math.Max(0, (int) Math.Log((playersCount - (i + 1)), 2));
                match.IsFinal = (match.Round == actualRounds);// && ((actualRounds == 1) || (i < playersCount - 1));
                if (section == CompetitionSection.Final && match.Round == rounds - 1)
                {
                    match.IsSemiFinal = true;
                }
                match.Status = MatchStatus.Created;
                matches.Add(match);
            }
            var matchesByRound = matches.GroupBy(m => m.Round).ToArray();
            foreach (var roundMatches in matchesByRound)
            {
                var index = 0;
                foreach (var match in roundMatches)
                {
                    match.RoundRelativePosition = index++;
                }
            }
            return matches;
        }
    }
}
