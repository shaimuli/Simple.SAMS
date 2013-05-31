using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Competitions.Services;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Contracts.Competitions
{
    [ServiceContract(Namespace = Namespaces.Services)]
    public interface ICompetitionsManager
    {
        [OperationContract(IsOneWay = true)]
        void StartCompetition(int id);
        [OperationContract(IsOneWay = true)]
        void FinishCompetition(int id);

        [OperationContract(IsOneWay = true)]
        void Create(CreateCompetitionInfo competitionCreateInfo);
        
        [OperationContract(IsOneWay = true)]
        void UpdateCompetitionPlayers(int competitionId, string playersFileUrl);

        [OperationContract(IsOneWay = true)]
        void LoadCompetitions(string competitionsFileUrl);
        
        [OperationContract(IsOneWay = true)]
        void LoadCompetitionsPlayers(string competitionsPlayersFileUrl);

        [OperationContract]
        MatchHeaderInfo[] GetCompetitionMatches(CompetitionMatchesQuery matchesQuery);

        [OperationContract]
        void UpdateMatchScore(MatchScoreUpdateInfo[] scoreUpdateInfoItems);

        [OperationContract]
        void UpdateMatchStartTime(MatchStartTimeUpdateInfo[] startTimeUpdateInfo);

        [OperationContract]
        void RemovePlayer(int competitionId, int playerId, CompetitionPlayerStatus status, string reason);

        [OperationContract]
        void ReplacePlayer(int competitionId, int replacedPlayerId, int replacingPlayerId, CompetitionPlayerSource source, CompetitionPlayerStatus status, string reason);

        [OperationContract]
        void AddPlayerToCompetition(int competitionId, int playerId, CompetitionPlayerSource source, CompetitionSection section);

        [OperationContract]
        LoadCompetitionsValidationResult ValidateCompetitionsFile(string competitionsFileUrl);

        [OperationContract]
        void PositionCompetitionPlayers(int id, CompetitionSection section);
    }
}
