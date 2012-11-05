﻿using System.ServiceModel;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Contracts.Competitions
{
    [ServiceContract(Namespace = Namespaces.Services)]
    public interface ICompetitionsEngine
    {
        [OperationContract]
        void CreateCompetitionsMatches(CompetitionHeaderInfo[] competitions);

        [OperationContract]
        int CreateCompetition(CreateCompetitionInfo competitionCreateInfo);

        [OperationContract]
        void AddPlayersToCompetition(int competitionId, Player[] players);

        [OperationContract]
        Player[] GetCompetitionPlayers(string playersFileUrl);

        [OperationContract]
        CreateCompetitionInfo[] GetCompetitions(string competitionsFileUrl);

        [OperationContract]
        void ImportCompetitions(CreateCompetitionInfo[] competitions);

        [OperationContract]
        void UpdatePlayersPosition(int[] competitionIds);

        [OperationContract]
        void UpdateMatchScore(MatchScoreUpdateInfo scoreUpdateInfo);

        [OperationContract]
        void UpdateMatchStartTime(MatchStartTimeUpdateInfo startTimeUpdateInfo);

    }
}
