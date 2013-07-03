using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.CompetitionEngine;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts
{
    public class KnockoutMatchProvisioningEngine : IMatchProvisioningEngine
    {
        public MatchHeaderInfo[] BuildMatches(CompetitionType competitionType, CompetitionDetails competitionDetails)
        {
            var helper = new KnockoutMatchProvisioningEngineHelper();
            var pMatches = helper.ProvisionMatches(competitionType.PlayersCount,
                                                  competitionType.QualifyingToFinalPlayersCount,
                                                  competitionDetails.Players.Length);
            var fMatches = pMatches.Where(m => m.SectionCode == "MD").ToArray();
            var qMatches = pMatches.Where(m => m.SectionCode == "Q").ToArray();

            var rankedPlayersCount = competitionType.PlayersCount - competitionType.QualifyingToFinalPlayersCount;
            var qPlayersCount = competitionDetails.Players.Length - rankedPlayersCount;
            if (qPlayersCount <= competitionType.QualifyingToFinalPlayersCount)
            {
                qPlayersCount = 0;
            }
            var matches = new List<MatchHeaderInfo>();
            var finalPlayersCount = PlayersCountCalculator.CalculatePlayersCount(rankedPlayersCount);
            var qualifyingPlayersCount = PlayersCountCalculator.CalculatePlayersCount(qPlayersCount);
            var finalSectionMatches = CreateSectionMatches(finalPlayersCount, CompetitionSection.Final).ToArray();
            var qualifyingSectionMatches = CreateSectionMatches(qualifyingPlayersCount, CompetitionSection.Qualifying, competitionType.QualifyingToFinalPlayersCount).ToArray();
            var consolationSectionMatches = CreateConselationMatches(finalPlayersCount);

            for (var i = 0; i < fMatches.Length; i++)
            {
                finalSectionMatches[i].Player1Code = fMatches[i].Player1Code;
                finalSectionMatches[i].Player2Code = fMatches[i].Player2Code;
            }
            for (var i = 0; i < qMatches.Length; i++)
            {
                qualifyingSectionMatches[i].Player1Code = qMatches[i].Player1Code;
                qualifyingSectionMatches[i].Player2Code = qMatches[i].Player2Code;
            }

            matches.AddRange(qualifyingSectionMatches);
            matches.AddRange(finalSectionMatches);
            matches.AddRange(consolationSectionMatches);
            return matches.ToArray();
        }

        private IEnumerable<MatchHeaderInfo> CreateConselationMatches(int finalPlayersCount)
        {
            var section = CompetitionSection.Consolation;

            var playersCount = finalPlayersCount;
            var matches = new List<MatchHeaderInfo>();
            var round = 0;
            var position = 0;
            var roundRelativePosition = 0;
            var match = default(MatchHeaderInfo);
            for (var i = (playersCount / 4); i >= 4; i /= 2)
            {
                for (var x = 0; x < 2; x++)
                {
                    for (var j = 0; j < i; j++)
                    {
                        match = new MatchHeaderInfo();
                        match.Section = section;
                        match.Position = position++;
                        match.RoundRelativePosition = roundRelativePosition++;
                        match.Round = round;
                        match.Status = MatchStatus.Created;
                        matches.Add(match);
                    }
                    round++;
                    roundRelativePosition = 0;
                }


            }

            match = new MatchHeaderInfo();
            match.Section = section;
            match.Position = position++;
            match.Round = round;
            match.IsSemiFinal = true;
            match.Status = MatchStatus.Created;
            matches.Add(match);            
            match = new MatchHeaderInfo();
            match.Section = section;
            match.Position = position++;
            match.Round = round;
            match.IsSemiFinal = true;
            match.Status = MatchStatus.Created;
            matches.Add(match);
            round++;

            match = new MatchHeaderInfo();
            match.Section = section;
            match.Position = position++;
            match.Round = round;
            match.IsFinal = true;
            match.Status = MatchStatus.Created;
            matches.Add(match);            

            return matches;

        }

        private IEnumerable<MatchHeaderInfo> CreateSectionMatches(int playersCount, CompetitionSection section, int? qualifyingToNextSection = default(int?))
        {
            var actualPlayersCount = playersCount - qualifyingToNextSection.GetValueOrDefault();
            var rounds = (int)Math.Log((playersCount), 2);
            var actualRounds = rounds - (qualifyingToNextSection.GetValueOrDefault() > 0 ? (int)Math.Log(qualifyingToNextSection.GetValueOrDefault(), 2) : 0);
            var matches = new List<MatchHeaderInfo>(playersCount);

            for (int i = 0; i < actualPlayersCount; i++)
            {
                var match = new MatchHeaderInfo();
                match.Section = section;
                match.Position = i;
                match.Round = rounds - Math.Max(0, (int)Math.Log((playersCount - (i + 1)), 2));
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
