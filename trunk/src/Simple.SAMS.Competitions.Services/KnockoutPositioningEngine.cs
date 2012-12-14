using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.CompetitionEngine;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;

namespace Simple.SAMS.Competitions.Services
{
    public class KnockoutPositioningEngine : IPositioningEngine
    {
        private Random m_randomizer = new Random();
        private int GetRandomPosition(int minValue, int maxValue)
        {
            return m_randomizer.Next(minValue, maxValue);
        }

        public Contracts.Competitions.UpdatePlayerPositionInfo[] PositionPlayers(Contracts.Competitions.CompetitionDetails details, Contracts.Competitions.CompetitionType competitionType)
        {
            var orderedMatches =
                details.Matches.OrderBy(m => m.Position).Take(competitionType.PlayersCount/2).ToArray();
            var positioningEngine = new FinalPositioningEngine();
            var positioningParameters = new FinalPositioningParameters
                                            {
                                                PlayersCount = details.Type.PlayersCount,
                                                RankedPlayersCount = 2,
                                                Players = details.Players.Take(details.Type.PlayersCount).Select(p => new Simple.SAMS.CompetitionEngine.Player() { Id = p.Id, Rank = p.CompetitionRank }).ToArray()
                                            };
            var positioningResults = positioningEngine.Evaluate(positioningParameters);
            var matchIndex = 0;
            var positionIndex = 0;
            var items = positioningResults.Select(
                position =>
                    {
                        var result = default(UpdatePlayerPositionInfo);
                        if (position.PlayerId.HasValue)
                        {
                            result = new UpdatePlayerPositionInfo();
                            result.PlayerId = position.PlayerId.Value;
                            result.MatchId = orderedMatches[matchIndex].Id;
                            result.Position = positionIndex%2;
                        }
                        positionIndex++;
                        if (positionIndex%2 == 0)
                        {
                            matchIndex++;
                        }
                        
                        return result;
                    });
            
            var results = items.Where(item => item.IsNotNull()).ToArray();

            return results;
        }
    }
}
