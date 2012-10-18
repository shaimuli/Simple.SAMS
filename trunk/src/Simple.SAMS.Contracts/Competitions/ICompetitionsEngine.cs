using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Contracts.Competitions
{
    public interface ICompetitionsEngine
    {
        void CreateCompetitionsMatches(CompetitionHeaderInfo[] competitions);
        int CreateCompetition(CreateCompetitionInfo competitionCreateInfo);
        void AddPlayersToCompetition(int competitionId, Player[] players);
        Player[] GetCompetitionPlayers(string playersFileUrl);
        CreateCompetitionInfo[] GetCompetitions(string competitionsFileUrl);
        void ImportCompetitions(CreateCompetitionInfo[] competitions);
    }
}
