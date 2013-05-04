using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [ServiceContract(Namespace = Namespaces.Services)]
    public interface ICompetitionTypeRepository : IEntityRepository<CompetitionType>
    {
        [OperationContract]
        CompetitionType[] GetCompetitionTypes();

        [OperationContract]
        bool AreCompetitionTypesExist(int[] ids);
    }
}
