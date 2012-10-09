using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [ServiceContract(Namespace = Namespaces.Services)]
    public interface ICompetitionMatchesRepository
    {
        [OperationContract]
        void AddCompetitionMatches(int competitionId, MatchHeaderInfo[] matches);

    }
}
