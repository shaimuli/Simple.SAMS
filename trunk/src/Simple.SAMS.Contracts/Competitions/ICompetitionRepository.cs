﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [ServiceContract(Namespace = Namespaces.Services)]
    public interface ICompetitionRepository
    {
        [OperationContract]
        bool DoesCompetitionExists(int competitionId);

        [OperationContract]
        void AddPlayersToCompetition(int competitionId, PlayerInCompetition[] players);

        [OperationContract]
        int Create(CompetitionHeaderInfo headerInfo);

        [OperationContract]
        CompetitionHeaderInfo GetCompetition(int id);

        [OperationContract]
        CompetitionDetails GetCompetitionDetails(int id);

        [OperationContract]
        CompetitionsSearchResult SearchCompetitions(CompetitionSearchQuery searchQuery);

        void UpdateCompetitionsByReferenceId(CompetitionHeaderInfo[] competitions);
    } 

}
 