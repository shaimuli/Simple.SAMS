using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.CompetitionEngine
{
    public class QualifyingPositioningEngine
    {
        public CompetitionPosition[] Evaluate(QualifyingPositionParameters parameters)
        {
            
            var playersCount = parameters.PlayersCount;
            var positions = new List<CompetitionPosition>(playersCount);
            var players = parameters.Players.Take(playersCount).ToList();
            players.ForEach(
                player =>
                    {
                        if (player.Rank <= 0)
                        {
                            player.Rank = playersCount + 1;
                        }
                    });
            players.Sort((p1,p2) =>
                             {
                                 if (p1.Rank >  p2.Rank)
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
            var qEngine = new QualifyingDrawPositionEngine();
            positions.AddRange(Enumerable.Range(0, PlayersCountCalculator.CalculatePlayersCount(players.Count)).Select(i => new CompetitionPosition()));
            var ranks = qEngine.Rank(players.Count);
            var map = new Dictionary<string, Player>();
            for (var i = 0; i < players.Count; i++)
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

            //var position = 0;
            //var skip = parameters.PlayersCount / parameters.QualifyingCount;
            
            //var availablePositions = Enumerable.Range(0, playersCount).ToList();
            //var directRankedPositions = new List<int>();
            //var rankedPositions = new List<int>();
            //while (players.Count > 0 && position < playersCount)
            //{
            //    var player = players[0];
            //    players.RemoveAt(0);
            //    positions[position].PlayerId = player.Id;
            //    availablePositions.RemoveAt(availablePositions.IndexOf(position));
            //    directRankedPositions.Add(position + 1);
            //    position += skip;
            //}

            //var randomPositionGenerator =
            //        new RandomPositionGenerator(Enumerable.Range(0, parameters.QualifyingCount ).Select(i=> ((i+1)*skip) -1 ).ToArray());
            
            //while (players.Count > 0 && randomPositionGenerator.CanTake())
            //{
            //    var player = players[0];
            //    players.RemoveAt(0);
            //    position = randomPositionGenerator.Take();
            //    availablePositions.RemoveAt(availablePositions.IndexOf(position));
            //    rankedPositions.Add(position+1);
            //    positions[position].PlayerId = player.Id;
            //}

            //if (availablePositions.Count > 0)
            //{
            //    randomPositionGenerator =
            //        new RandomPositionGenerator(availablePositions.Except(directRankedPositions.Concat(rankedPositions)).ToArray());

            //    while (players.Count > 0 && randomPositionGenerator.CanTake())
            //    {
            //        var player = players[0];
            //        players.RemoveAt(0);
            //        position = randomPositionGenerator.Take();
            //        availablePositions.RemoveAt(availablePositions.IndexOf(position));
            //        positions[position].PlayerId = player.Id;
            //    }
            //}
            
            //if (availablePositions.Count > 0)
            //{
            //    randomPositionGenerator =
            //        new RandomPositionGenerator(
            //            availablePositions.Except(directRankedPositions).ToArray());

            //    while (players.Count > 0 && randomPositionGenerator.CanTake())
            //    {
            //        var player = players[0];
            //        players.RemoveAt(0);
            //        position = randomPositionGenerator.Take();
            //        availablePositions.RemoveAt(availablePositions.IndexOf(position));
            //        positions[position].PlayerId = player.Id;
            //    }
            //}
            
            //if (availablePositions.Count > 0)
            //{
            //    availablePositions = availablePositions.Skip(availablePositions.Count - players.Count).ToList();

            //    randomPositionGenerator =
            //        new RandomPositionGenerator(availablePositions.ToArray());

            //    while (players.Count > 0 && randomPositionGenerator.CanTake())
            //    {
            //        var player = players[0];
            //        players.RemoveAt(0);
            //        position = randomPositionGenerator.Take();
            //        availablePositions.RemoveAt(availablePositions.IndexOf(position));
            //        positions[position].PlayerId = player.Id;
            //    }
            //}
            

            return positions.ToArray();
        }
    }
}
