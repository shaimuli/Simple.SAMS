using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var players = details.Players.Take(competitionType.PlayersCount).ToList();
            var maxPlayerRank = players.Max(p => p.CompetitionRank);
            players.ForEach(
                p=>
                    {
                        if (p.CompetitionRank <= 0)
                        {
                            p.CompetitionRank = ++maxPlayerRank;
                        }
                    });
            var positions = new List<UpdatePlayerPositionInfo>();
            var rankedPlayers = players.OrderBy(p=>p.CompetitionRank).ToList();
            var wildCardPlayers = rankedPlayers.Take(competitionType.WildcardPlayersCount).ToList();
            if (rankedPlayers.Count > 0)
            {
                var orderedMatches =
                    details.Matches.OrderBy(m => m.Position).Take(competitionType.PlayersCount/2).ToList();


                 positions =
                    orderedMatches.SelectMany(m =>
                                              new[]
                                                  {
                                                      new UpdatePlayerPositionInfo()
                                                          {
                                                              MatchId = m.Id,
                                                              Position = 0
                                                          },
                                                      new UpdatePlayerPositionInfo()
                                                          {
                                                              MatchId = m.Id,
                                                              Position = 1
                                                          }
                                                  }).ToList();
                positions.ForEach(
                    position =>
                        {
                            var playerIndex = GetRandomPosition(0, rankedPlayers.Count - 1);
                            position.PlayerId = rankedPlayers[playerIndex].Id;
                            rankedPlayers.RemoveAt(playerIndex);
                        });

                if (wildCardPlayers.Count > 0)
                {
                    var rankedPositions = new[] {0, positions.Count - 1};
                    wildCardPlayers = PositionWildcardPlayers(rankedPositions, wildCardPlayers.ToArray(), positions);
                }

                if (wildCardPlayers.Count > 0 && positions.Count == 16)
                {
                    var rankedPositions = new[] {4, 11};
                    wildCardPlayers = PositionWildcardPlayers(rankedPositions, wildCardPlayers.Take(2).ToArray(),
                                                              positions, true);
                }
                if (wildCardPlayers.Count > 0 && positions.Count == 32)
                {
                    var rankedPositions = new[] {7, 15, 16, 24};
                    PositionWildcardPlayers(rankedPositions, wildCardPlayers.Take(4).ToArray(), positions, true);
                }
            }
            //var matchIndex = 0;
            //while (players.Count>0)
            //{
            //    var player = players[0];
            //    if (matchIndex>orderedMatches.Count)
            //    {
            //        throw new ApplicationException("Matches count is insufficient.");
            //    }
            //    var match = orderedMatches[matchIndex];
            //    var position = 0;
                
            //    if (match.Player1 == null)
            //    {
            //        match.Player1 = new MatchPlayer() {Id = player.Id};
            //    }
            //    else if (match.Player2 == null)
            //    {
            //        match.Player2 = new MatchPlayer() { Id = player.Id };
            //        position = 1;
            //        matchIndex++;
            //    }

            //    var updateInfo = new UpdatePlayerPositionInfo() { MatchId = match.Id, PlayerId = player.Id, Position = position };
            //    positions.Add(updateInfo);
            //    players.RemoveAt(0);
            //}

            //var finalMatches = orderedMatches.Where(m => m.Section == CompetitionSection.Final);
            //var qualifyingMatches = orderedMatches.Where(m => m.Section == CompetitionSection.Qualifying);

            
            //if (competitionType.WildcardPlayersCount > 0)
            //{
            //    var topPlayers =
            //        players.OrderBy(p => p.CompetitionRank).Take(competitionType.WildcardPlayersCount);

            //    //positions.Add(new UpdatePlayerPositionInfo(){ MatchId = finalMatches.First().Id, PlayerId = topPlayers.First() })
                

            //    // TBD

            //    //var firstMatch = finalMatches.FirstOrDefault();
            //    //if (firstMatch.IsNull())
            //    //{
            //    //    throw new ApplicationException("At least one match is required in final section when wildcard players count larger than 0.");
            //    //}
            //    //var firstPlayer = topPlayers.FirstOrDefault();
            //    //if (firstPlayer.IsNull())
            //    //{
            //    //    throw new ApplicationException("At least one player is required in final section when wildcard players count larger than 0.");
            //    //}

            //    //positions.Add(new UpdatePlayerPositionInfo(){ MatchId = firstMatch.Id, PlayerId = firstPlayer.Id });


            //}

            return positions.ToArray();
        }

        private List<CompetitionPlayer> PositionWildcardPlayers(int[] rankedPositions, CompetitionPlayer[] wildCardPlayers, List<UpdatePlayerPositionInfo> positions, bool randomize = false)
        {
            var result = new List<CompetitionPlayer>(wildCardPlayers);
            foreach (var rankedPosition in rankedPositions)
            {
                if (result.Count > 0)
                {
                    var playerIndex = 0;
                    if (randomize)
                    {
                        playerIndex = GetRandomPosition(0, result.Count - 1);
                    }
                    var wildCardPlayer = result[playerIndex];
                    result.RemoveAt(playerIndex);
                    var rankPosition = positions[rankedPosition];
                    var wildCardPlayerPosition = positions.First(p => p.PlayerId == wildCardPlayer.Id);
                    var original = rankPosition.PlayerId;
                    rankPosition.PlayerId = wildCardPlayer.Id;
                    wildCardPlayerPosition.PlayerId = original;
                    playerIndex++;
                }
            }

            return result;
        }
    }
}
