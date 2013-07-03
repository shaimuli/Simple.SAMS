using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnockoutEngine
{
    public class KnockoutCalculationOutput
    {
        public int ActualMainDrawPlayers { get; set; }
        public int ActualQualifyingPlayers { get; set; }
        public int TotalMainDrawPlayers { get; set; }
        public int TotalQualifyingPlayers { get; set; }
        public int MainDrawMatchesCount { get; set; }
        public int QualifyingDrawMatchesCount { get; set; }
    }

    public class KnockoutCalculationEngine
    {

        public int CalculatePlayersCount(int actualPlayersCount)
        {
            var playersCount = 0;
            if (actualPlayersCount > 0)
            {
                if (actualPlayersCount <= 2)
                {
                    playersCount = 2;
                }
                else if (actualPlayersCount <= 4)
                {
                    playersCount = 4;
                }
                else if (actualPlayersCount <= 8)
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
            }
            return playersCount;
        }
        public KnockoutCalculationOutput Calculate(int playersCount, int qualifyingPlayersCount, int actualPlayersCount)
        {
            var mainDrawPlayersCount = playersCount - qualifyingPlayersCount;
            var qualifyingDrawPlayersCount = 0;
            if (qualifyingPlayersCount > 0)
            {
                if (actualPlayersCount > mainDrawPlayersCount)
                {
                    qualifyingDrawPlayersCount = actualPlayersCount - mainDrawPlayersCount;
                }
                if (qualifyingDrawPlayersCount <= qualifyingPlayersCount)
                {
                    mainDrawPlayersCount += qualifyingDrawPlayersCount;
                    qualifyingDrawPlayersCount = 0;
                    
                }
            }
            mainDrawPlayersCount = Math.Min(mainDrawPlayersCount, actualPlayersCount - qualifyingDrawPlayersCount);
            var mainDrawActualPlayersCount = CalculatePlayersCount(mainDrawPlayersCount + (qualifyingDrawPlayersCount>0?qualifyingPlayersCount:0));
            var qualifyingDrawActualPlayersCount = CalculatePlayersCount(qualifyingDrawPlayersCount);

            var output = new KnockoutCalculationOutput()
            {
                MainDrawMatchesCount = mainDrawActualPlayersCount,
                QualifyingDrawMatchesCount = CalcMatchesCount(qualifyingDrawActualPlayersCount, qualifyingPlayersCount),
                ActualMainDrawPlayers = mainDrawPlayersCount,
                ActualQualifyingPlayers = qualifyingDrawPlayersCount,
                TotalMainDrawPlayers = mainDrawActualPlayersCount,
                TotalQualifyingPlayers = qualifyingDrawActualPlayersCount
            };

            return output;
        }

        private int CalcMatchesCount(int start, int qualifying)
        {
            var list = new List<int>();
            list.Add(start/2);
            while (list.Last() > qualifying)
            {
                list.Add(list.Last()/2);
            }

            return list.Sum();
        }
    }
}
