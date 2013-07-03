using System.Collections.Generic;
using System.Linq;

namespace Simple.SAMS.CompetitionEngine
{
    public class QualifyingDrawPositionEngine
    {
        public int CalculatePlayersCount(int actualPlayersCount)
        {
            var playersCount = 0;
            if (actualPlayersCount <= 8)
            {
                playersCount = 8;
            }
            else if (actualPlayersCount <= 16)
            {
                playersCount = 16;
            }
            else if (actualPlayersCount <= 32)
            {
                playersCount = 32;
            }
            else if (actualPlayersCount <= 64)
            {
                playersCount = 64;
            }
            else if (actualPlayersCount <= 128)
            {
                playersCount = 128;
            }
            return playersCount;
        }

        public PositioningItem[] Rank(int actualPlayersCount)
        {
            var playersCount = CalculatePlayersCount(actualPlayersCount);
            
                        var codes = new Queue<string>();
            for (var i = 0; i < actualPlayersCount ; i++)
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

            var items = new List<PositioningItem>(playersCount);
            var skip = playersCount / 8;
            var first = new List<int>();
            var second = new List<int>();
            var third = new List<int>();

            for (var i = 0; i < playersCount; i += skip)
            {
                items[i] = new PositioningItem() { Index = i, Code = codes.Dequeue() };
                first.Add(i);
                second.Add(i + 2);
                second.Add(i + 4);
                third.Add(i + 3);
                third.Add(i + 5);
            }
            var oddPlaces = new List<int>();
            for (var i = skip - 1; i < playersCount; i += skip)
            {
                oddPlaces.Add(i);
            }
            first.AddRange(oddPlaces);

            var randomizer = new RandomPositionGenerator(oddPlaces.ToArray());
            while (randomizer.CanTake())
            {
                var i = randomizer.Take();
                items[i] = new PositioningItem() { Index = i, Code = codes.Dequeue() };
            }

            randomizer = new RandomPositionGenerator(second.ToArray());
            while (randomizer.CanTake())
            {
                var i = randomizer.Take();
                items[i] = new PositioningItem() { Index = i, Code = codes.Dequeue() };
            }

            randomizer = new RandomPositionGenerator(third.ToArray());
            while (randomizer.CanTake())
            {
                var i = randomizer.Take();
                items[i] = new PositioningItem() { Index = i, Code = codes.Dequeue() };
            }

            var available = Enumerable.Range(0, playersCount).Except(first).Except(second).Except(third);
            randomizer = new RandomPositionGenerator(available.ToArray());
            while (randomizer.CanTake())
            {
                var i = randomizer.Take();
                items[i] = new PositioningItem() { Index = i, Code = codes.Dequeue() };
            }

            
            

            //for (int i = 0; i < playersCount; i++)
            //{
            //    var item = new PositioningItem() {Index = i};
            //    if (i%2 == 0)
            //    {
            //        item.Code = codes.Dequeue();
            //    }
            //    items.Add(item);
            //}



            //var empty = items.Where(i => string.IsNullOrEmpty(i.Code));
            //var randomizer = new RandomPositionGenerator(empty.Select(i=>i.Index).ToArray());
            //while (randomizer.CanTake())
            //{
            //    items[randomizer.Take()].Code = codes.Dequeue();
            //}

            return items.ToArray();
        }
    }
}
