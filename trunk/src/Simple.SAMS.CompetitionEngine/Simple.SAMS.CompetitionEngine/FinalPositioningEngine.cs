using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.CompetitionEngine
{
    public class FinalPositioningEngine
    {
        public CompetitionPosition[] Evaluate(FinalPositioningParameters parameters)
        {
            var playersCount = parameters.Players.Length;
            var positions = new List<CompetitionPosition>(playersCount);
            
            if (playersCount > 0)
            {
                var players = parameters.Players.Take(playersCount).ToList();
                players.ForEach(
                     player =>
                     {
                         if (player.Rank <= 0)
                         {
                             player.Rank = playersCount + 1;
                         }
                     });
                players.Sort((p1, p2) =>
                {
                    if (p1.Rank > p2.Rank)
                    {
                        return 1;
                    }
                    else if (p2.Rank > p1.Rank)
                    {
                        return -1;
                    }
                    else
                    {
                        if (p1.AverageScore > p2.AverageScore)
                        {
                            return 1;
                        }
                        else if (p2.AverageScore > p1.AverageScore)
                        {
                            return -1;
                        }
                        else
                        {
                            if (p1.AccumulatedScore > p2.AccumulatedScore)
                            {
                                return 1;
                            }
                            else if (p2.AccumulatedScore > p1.AccumulatedScore)
                            {
                                return -1;
                            }
                            else
                            {
                                return 0;
                            }
                        }

                    }
                });

                var mdEngine = new MainDrawPositionEngine();
                positions.AddRange(Enumerable.Range(0, PlayersCountCalculator.CalculatePlayersCount(players.Count)).Select(i => new CompetitionPosition()));
                var ranks = mdEngine.Rank(players.Count, parameters.QualifyingToFinal);
                var map = new Dictionary<string, Player>();
                for (var i = 0; i < players.Count - parameters.QualifyingToFinal; i++)
                {
                    map["MD" + (i + 1)] = players[i];
                }

                for (int i = players.Count - parameters.QualifyingToFinal; i < players.Count; i++)
                {
                    map["Q" + (i + 1)] = players[i];
                }

                foreach (var item in ranks)
                {
                    Player player;
                    if (map.TryGetValue(item.Code, out player))
                    {
                        var position = new CompetitionPosition
                                           {
                                               PlayerId = player.Id
                                           };

                        positions[item.Index] = position;
                    }
                }

                //var availablePositions = Enumerable.Range(0, playersCount).ToList();
                //var rankGroups = new List<List<int>>()
                //                     {
                //                         new List<int>()
                //                     };
                //var rankedPositions = new List<int>();
                //var position = 0;

                //RankPlayer(positions, position, players, availablePositions, rankGroups[0]);

                //if (players.Count > 0)
                //{
                //    position = playersCount - 1;
                //    RankPlayer(positions, position, players, availablePositions, rankGroups[0]);
                //}

                //if (parameters.QualifyingToFinal > 2)
                //{
                //    rankGroups.Add(new List<int>());
                //    RankPlayers(positions, new[] { parameters.QualifyingToFinal, (playersCount - parameters.QualifyingToFinal-1) }, players, availablePositions, rankGroups[1]);
                //}

                //if (parameters.QualifyingToFinal > 4)
                //{
                //    rankGroups.Add(new List<int>());
                //    var rndpositions = new[]
                //                           {
                //                               parameters.QualifyingToFinal - 1,
                //                               parameters.QualifyingToFinal*2 - 1,
                //                               (playersCount - parameters.QualifyingToFinal*2),
                //                               playersCount - parameters.QualifyingToFinal
                //                           };
                //    RankPlayers(positions, rndpositions, players, availablePositions, rankGroups[2]);
                //}

                //if (parameters.QualifyingToFinal > 8)
                //{
                //    throw new NotSupportedException("Only up to 32->8");
                //    rankGroups.Add(new List<int>());
                //    var rndpositions = new[]
                //                           {
                //                               parameters.QualifyingToFinal - 1,
                //                               parameters.QualifyingToFinal*2 - 1,
                //                               (playersCount - parameters.QualifyingToFinal*2),
                //                               playersCount - parameters.QualifyingToFinal
                //                           };
                //    RankPlayers(positions, rndpositions, players, availablePositions, rankGroups[3]);
                //}

                //var set1 = availablePositions.Except(rankGroups.SelectMany(i=>i.ToList())).ToList();
                //RankPlayers(positions, set1.ToArray(), players, availablePositions, null);
                //for(var groupIndex = rankGroups.Count - 1; groupIndex >=0; groupIndex--)
                //{
                //    var set = availablePositions.Except(rankGroups[groupIndex]).ToList();
                //    RankPlayers(positions, set1.ToArray(), players, availablePositions, null);
                //}

                //RankPlayers(positions, availablePositions.ToArray(), players, availablePositions, null);

            }

            return positions.ToArray();

        }

        private void RankPlayers(List<CompetitionPosition> positions, int[] randomPositions, List<Player> players, List<int> availablePositions,
                                       List<int> rankedPositions)
        {
            var randomizer = new RandomPositionGenerator(randomPositions);
            while (players.Count > 0 && randomizer.CanTake())
            {
                var position = randomizer.Take();
                RankPlayer(positions, position, players, availablePositions, rankedPositions);
            }
        }
        private void RankPlayer(List<CompetitionPosition> positions, int position, List<Player> players, List<int> availablePositions,
                                       List<int> directRankedPositions)
        {
            positions[position].PlayerId = players[0].Id;
            players.RemoveAt(0);
            availablePositions.RemoveAt(availablePositions.IndexOf(position));
            if (directRankedPositions != null)
            {
                directRankedPositions.Add(position);
            }
        }
    }
}
