using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
        void Create(CreateCompetitionInfo competitionCreateInfo, string playersFile);
        
        [OperationContract(IsOneWay = true)]
        void UpdateCompetitionPlayers(int competitionId, string playersFileUrl);

        [OperationContract(IsOneWay = true)]
        void LoadCompetitions(string competitionsFileUrl);

        [OperationContract]
        MatchHeaderInfo[] GetCompetitionMatches(CompetitionMatchesQuery matchesQuery);

        [OperationContract]
        void UpdateMatchScore(MatchScoreUpdateInfo[] scoreUpdateInfoItems);

        [OperationContract]
        void UpdateMatchStartTime(MatchStartTimeUpdateInfo[] startTimeUpdateInfo);

    }
}
