using System.Collections.Generic;

namespace Simple.SAMS.Contracts
{
    public class CompetitionPlayerComparer : IComparer<Contracts.Players.CompetitionPlayer>
    {

        public int Compare(Players.CompetitionPlayer p1, Contracts.Players.CompetitionPlayer p2)
        {
            if (p1.CompetitionRank > p2.CompetitionRank)
            {
                return 1;
            }
            else if (p2.CompetitionRank > p1.CompetitionRank)
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
        }
    }
}
