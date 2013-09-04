using System;
using System.Collections.Generic;
using System.Linq;
using KnockoutEngine;
using Simple.SAMS.CompetitionEngine;

namespace Simple.SAMS.Contracts
{
    public class KnockoutMatchProvisioningEngineHelper
    {
        public MatchInfo[] ProvisionMatches(int playersCount, int qualifyingPlayersCount, int actualPlayersCount)
        {
            var engine = new KnockoutCalculationEngine();
            var output = engine.Calculate(playersCount, qualifyingPlayersCount, actualPlayersCount);
            var matches = new List<MatchInfo>(output.MainDrawMatchesCount + output.QualifyingDrawMatchesCount);

            matches.AddRange(CreateMainDrawMatches(output.MainDrawMatchesCount, output.ActualMainDrawPlayers, output.TotalMainDrawPlayers, output.ActualQualifyingPlayers));
            if (output.TotalQualifyingPlayers > 0)
            {
                matches.AddRange(CreateQualifyingMatches(output.QualifyingDrawMatchesCount,
                                                         output.TotalQualifyingPlayers, output.ActualQualifyingPlayers));
            }

            return matches.ToArray();
        }

        private IEnumerable<MatchInfo> CreateQualifyingMatches(int qualifyingDrawMatchesCount, int playersCount, int actualPlayersCount)
        {

            var mNumbers = Enumerable.Range(0, qualifyingDrawMatchesCount);
            var matches = mNumbers.Select(i => new MatchInfo()
            {
                Index = i,
                SectionCode = "Q"
            }).ToArray();

            var codes = GenerateQualifyingDrawCodes(playersCount, actualPlayersCount);
            PositionQualifyingDraw(matches, playersCount, actualPlayersCount, codes);
            return matches;
        }

        private void PositionQualifyingDraw(MatchInfo[] matches, int playersCount, int actualPlayersCount, Queue<string> codes)
        {

            var skip = playersCount / 8;
            var first = new List<int>();
            var second = new List<int>();
            var third = new List<int>();
            var against = new Stack<int>();
            for (var i = 0; i < playersCount; i += skip)
            {
                PositionInMatch(matches, i, codes.Dequeue());
                against.Push(i + 1);
                if (skip > 1)
                {
                    first.Add(i);
                }
                if (skip > 4)
                {
                    for (var j = 2; j < skip; j *= 2)
                    {
                        second.Add(i + j);
                        third.Add(i + j + 1);
                    }
                }
            }
            var oddPlaces = new List<int>();

            if (codes.Count > 0)
            {
                if (skip > 2)
                {
                    for (var i = skip - 1; i < playersCount; i += skip)
                    {
                        oddPlaces.Add(i);
                        against.Push(i - 1);
                    }
                    first.AddRange(oddPlaces);
                    var randomizer = new RandomPositionGenerator(oddPlaces.ToArray());
                    while (randomizer.CanTake())
                    {
                        PositionInMatch(matches, randomizer.Take(), codes.Dequeue());
                    }


                    randomizer = new RandomPositionGenerator(second.ToArray());
                    while (randomizer.CanTake())
                    {
                        PositionInMatch(matches, randomizer.Take(), codes.Dequeue());
                    }

                    randomizer = new RandomPositionGenerator(third.ToArray());
                    while (randomizer.CanTake())
                    {
                        PositionInMatch(matches, randomizer.Take(), codes.Dequeue());
                    }
                }

                var available = Enumerable.Range(0, playersCount).Except(first).Except(second).Except(third);
                if (available.Any() && codes.Count > 0)
                {
                    var remainderQueue = new Queue<string>();
                    var code = codes.Dequeue();
                    while (against.Count > 0 && code != "BYE")
                    {
                        against.Pop();
                        remainderQueue.Enqueue(code);
                        code = codes.Dequeue();

                    }
                    if (against.Count > 0)
                    {
                        available = available.Except(against);
                    }
                    if (available.Any() && remainderQueue.Count > 0)
                    {
                        var randomizer = new RandomPositionGenerator(available.ToArray());
                        while (randomizer.CanTake())
                        {
                            PositionInMatch(matches, randomizer.Take(), remainderQueue.Dequeue());
                        }
                    }

                    while (against.Count > 0)
                    {
                        PositionInMatch(matches, against.Pop(), "BYE");
                    }
                }
            }
        }

        public Queue<string> GenerateQualifyingDrawCodes(int playersCount, int actualPlayersCount)
        {
            var codes = new Queue<string>();
            for (var i = 0; i < actualPlayersCount; i++)
            {
                codes.Enqueue("Q" + (i + 1));
            }


            if (actualPlayersCount < playersCount)
            {
                for (var i = 0; i < (playersCount - actualPlayersCount); i++)
                {
                    codes.Enqueue("BYE");
                }
            }
            return codes;
        }

        private IEnumerable<MatchInfo> CreateMainDrawMatches(int mainDrawMatchesCount, int actualPlayersCount, int playersCount, int qualifyingPlayersCount)
        {
            var mNumbers = Enumerable.Range(0, mainDrawMatchesCount);
            var matches = mNumbers.Select(i => new MatchInfo()
            {
                Index = i,
                SectionCode = "MD"
            }).ToArray();

            var codes = GenerateMainDrawCodesQueue(playersCount, actualPlayersCount, qualifyingPlayersCount);
            var rankedPlayers = Math.Max(2, mainDrawMatchesCount / 4);
            PositionMainDraw(rankedPlayers, matches, codes);
            return matches;
        }

        private void PositionInMatch(MatchInfo[] matches, int index, string code)
        {
            var match = matches[index / 2];
            if (index % 2 == 0)
            {
                match.Player1Code = code;
            }
            else
            {
                match.Player2Code = code;
            }
        }

        private void PositionMainDraw(int rankedPlayers, MatchInfo[] matches, Queue<string> codes)
        {
            var playersCount = matches.Length;
            var vsRanked = new Stack<int>();
            var taken = new List<int>();

            PositionInMatch(matches, 0, codes.Dequeue());
            PositionInMatch(matches, playersCount - 1, codes.Dequeue());
            taken.Add(0);
            taken.Add(playersCount - 1);
            vsRanked.Push(1);
            vsRanked.Push(matches.Length - 2);

            var randomizer = default(RandomPositionGenerator);
            if (rankedPlayers > 2)
            {
                var p1 = playersCount / 4;
                var p2 = (playersCount - 1) - playersCount / 4;
                randomizer = new RandomPositionGenerator(new[] { p1, p2 });
                taken.Add(randomizer.Take());
                PositionInMatch(matches, taken.Last(), codes.Dequeue());

                taken.Add(randomizer.Take());
                PositionInMatch(matches, taken.Last(), codes.Dequeue());

                vsRanked.Push(p1 + 1);
                vsRanked.Push(p2 - 1);
            }

            if (rankedPlayers > 4)
            {
                var p = new[]
                            {
                                playersCount / 2-1,
                                playersCount/2,
                                playersCount / 4-1,
                                (playersCount) - playersCount/4,

                            };
                randomizer = new RandomPositionGenerator(p);

                taken.Add(randomizer.Take());
                PositionInMatch(matches, taken.Last(), codes.Dequeue());
                taken.Add(randomizer.Take());
                PositionInMatch(matches, taken.Last(), codes.Dequeue());
                taken.Add(randomizer.Take());
                PositionInMatch(matches, taken.Last(), codes.Dequeue());
                taken.Add(randomizer.Take());
                PositionInMatch(matches, taken.Last(), codes.Dequeue());


                vsRanked.Push(p[0] - 1);
                vsRanked.Push(p[1] + 1);
                vsRanked.Push(p[2] - 1);
                vsRanked.Push(p[3] + 1);
            }



            var positions = Enumerable.Range(0, playersCount).Except(vsRanked).Except(taken).ToList();
            randomizer = new RandomPositionGenerator(positions.ToArray());

            while (randomizer.CanTake())
            {
                PositionInMatch(matches, randomizer.Take(), codes.Dequeue());
            }

            if (rankedPlayers > 4)
            {
                positions = new List<int>();
                positions.Add(vsRanked.Pop());
                positions.Add(vsRanked.Pop());
                positions.Add(vsRanked.Pop());
                positions.Add(vsRanked.Pop());
                randomizer = new RandomPositionGenerator(positions.ToArray());
                while (randomizer.CanTake())
                {
                    PositionInMatch(matches, randomizer.Take(), codes.Dequeue());
                }
            }

            if (rankedPlayers > 2)
            {
                positions = new List<int>();
                positions.Add(vsRanked.Pop());
                positions.Add(vsRanked.Pop());
                randomizer = new RandomPositionGenerator(positions.ToArray());
                while (randomizer.CanTake())
                {
                    PositionInMatch(matches, randomizer.Take(), codes.Dequeue());
                }
            }

            PositionInMatch(matches, vsRanked.Pop(), codes.Dequeue());
            PositionInMatch(matches, vsRanked.Pop(), codes.Dequeue());
        }

        public Queue<string> GenerateMainDrawCodesQueue(int totalPlayersCount, int actualPlayersCount, int qualifyingPlayersCount)
        {
            var codes = new Queue<string>();
            for (var i = 0; i < actualPlayersCount; i++)
            {
                codes.Enqueue("MD" + (i + 1));
            }

            for (int i = 0; i < qualifyingPlayersCount; i++)
            {
                codes.Enqueue("Q" + (i + 1));
            }
            if (codes.Count < totalPlayersCount)
            {
                for (var i = 0; i < (totalPlayersCount - actualPlayersCount); i++)
                {
                    codes.Enqueue("BYE");
                }
            }
            return codes;
        }
    }
}
