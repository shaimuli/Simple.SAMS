namespace Simple.SAMS.CompetitionEngine
{
    public static class PlayersCountCalculator
    {
        public static int CalculatePlayersCount(int actualPlayersCount)
        {
            var playersCount = 0;
            if (actualPlayersCount > 0)
            {
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
            }
            return playersCount;
        }    
    }
}