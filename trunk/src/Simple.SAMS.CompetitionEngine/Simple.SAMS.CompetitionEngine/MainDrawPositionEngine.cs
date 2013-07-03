using System.Collections.Generic;
using System.Linq;

namespace Simple.SAMS.CompetitionEngine
{
    public class MainDrawPositionEngine
    {


        public PositioningItem[] Rank(int actualPlayersCount, int qPlayers)
        {
            var playersCount = PlayersCountCalculator.CalculatePlayersCount(actualPlayersCount);
            var rankedPlayers = playersCount/4;
            

            var items = new List<PositioningItem>(playersCount);
            for (int i = 0; i < playersCount; i++)
            {
                items.Add(new PositioningItem() { Index = i });
            }

            var codes = new Queue<string>();
            for (var i = 0; i < actualPlayersCount - qPlayers; i++)
            {
                codes.Enqueue("MD" + (i + 1));
            }

            for (int i = 0; i < qPlayers; i++)
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

            var vsRanked = new Stack<int>();

            items[0].Code = codes.Dequeue();
            items.Last().Code = codes.Dequeue();
            vsRanked.Push(1);
            vsRanked.Push(items.Count - 2);

            var randomizer = default(RandomPositionGenerator);
            if (rankedPlayers > 2)
            {
                var p1 = items.Count / 4;
                var p2 = (items.Count - 1) - items.Count / 4;
                randomizer = new RandomPositionGenerator(new[] { p1, p2 });
                items[randomizer.Take()].Code = codes.Dequeue();
                items[randomizer.Take()].Code = codes.Dequeue();
                vsRanked.Push(p1 + 1);
                vsRanked.Push(p2 - 1);
            }

            if (rankedPlayers > 4)
            {
                var p = new[]
                            {
                                items.Count / 2-1,
                                items.Count/2,
                                items.Count / 4-1,
                                (items.Count) - items.Count/4,

                            };
                randomizer = new RandomPositionGenerator(p);
                items[randomizer.Take()].Code = codes.Dequeue();
                items[randomizer.Take()].Code = codes.Dequeue();
                items[randomizer.Take()].Code = codes.Dequeue();
                items[randomizer.Take()].Code = codes.Dequeue();

                vsRanked.Push(p[0] - 1);
                vsRanked.Push(p[1] + 1);
                vsRanked.Push(p[2] - 1);
                vsRanked.Push(p[3] + 1);
            }


            var emptySlots = items.Where(item => string.IsNullOrEmpty(item.Code));
            var positions = emptySlots.Select(i => i.Index).Except(vsRanked).ToList();
            randomizer = new RandomPositionGenerator(positions.ToArray());

            while (randomizer.CanTake())
            {
                items[randomizer.Take()].Code = codes.Dequeue();
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
                    items[randomizer.Take()].Code = codes.Dequeue();
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
                    items[randomizer.Take()].Code = codes.Dequeue();
                }
            }

            items[vsRanked.Pop()].Code = codes.Dequeue();
            items[vsRanked.Pop()].Code = codes.Dequeue();

            return items.ToArray();
        }

        


    }
}
