using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts
{
    public interface IEntityRepository<TEntity> where TEntity: IEntity
    {
        [OperationContract]
        TEntity Get(int id);

        [OperationContract]
        void Remove(int id);

        [OperationContract]
        int Add(TEntity entity);

        [OperationContract]
        void Update(TEntity entity);
    }
}
