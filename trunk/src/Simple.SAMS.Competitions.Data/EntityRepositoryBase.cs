using System;
using System.Data.Linq;
using Simple.SAMS.Contracts;
using Simple.Utilities;

namespace Simple.SAMS.Competitions.Data
{
    public abstract class EntityRepositoryBase<TEntity, TDataEntity, TDataContext> : DataRepositoryBase<TDataContext>, IEntityRepository<TEntity>
        where TEntity : class, IEntity
        where TDataEntity : class, IDataEntity
        where TDataContext : DataContext, new()
    {
        public TEntity Get(int id)
        {
            var result = default(TEntity);

            UseDataContext(
                dataContext =>
                    {
                        var dataEntity = GetById(dataContext, id);
                        if (dataEntity.IsNotNull())
                        {
                            var entity = CreateEntity(dataEntity);
                        
                            entity.Id = dataEntity.Id;
                            entity.Created = dataEntity.Created;
                            entity.Updated = dataEntity.Updated;
                            entity.ItemStatus = (EntityItemStatus) dataEntity.RowStatus;

                            MapDataEntityToEntity(dataEntity, entity);
                            result = entity;
                        }
                    });

            return result;
        }

        public void Remove(int id)
        {
            Requires.IntArgumentPositive(id, "id");

            UseDataContext(
                dataContext =>
                    {
                        var dataEntity = GetById(dataContext, id);
                        if (dataEntity.IsNull())
                        {
                            throw new ArgumentException("Cannot find entity '{0}' with id '{1}'".ParseTemplate(typeof(CompetitionType).Name, id));
                        }

                        dataEntity.RowStatus = (int) EntityItemStatus.Deleted;
                        dataContext.SubmitChanges();
                    });
        }

        protected abstract TDataEntity GetById(TDataContext dataContext, int id);
        protected abstract TEntity CreateEntity(TDataEntity dataEntity);
        protected abstract void MapDataEntityToEntity(TDataEntity dataEntity, TEntity entity);
        protected abstract TDataEntity CreateDataEntity(TEntity entity);
        protected abstract void MapEntityToDataEntity(TEntity entity, TDataEntity dataEntity);
        protected abstract void InsertEntity(TDataContext dataContext, TDataEntity dataEntity);
        

        public int Add(TEntity entity)
        {
            Requires.ArgumentNotNull(entity, "entity");
            if (entity.Id != 0)
            {
                throw new ArgumentException("You cannot add existing entity.");
            }
            var result = default(int);
            UseDataContext(
                dataContext =>
                    {
                        var dataEntity = CreateDataEntity(entity);
                        MapEntityToDataEntity(entity, dataEntity);
                        SetNewDataEntityParameters(dataEntity);
                        InsertEntity(dataContext, dataEntity);

                        dataContext.SubmitChanges();

                        result = dataEntity.Id;
                    });

            return result;
        }

        protected void SetNewDataEntityParameters(TDataEntity dataEntity)
        {
            dataEntity.Created = dataEntity.Updated = DateTime.UtcNow;
            dataEntity.RowStatus = (int) EntityItemStatus.Active;
        }

        public void Update(TEntity entity)
        {
            Requires.ArgumentNotNull(entity, "entity");
            Requires.IntArgumentPositive(entity.Id, "entity.Id");

            UseDataContext(
                dataContext =>
                    {
                        var dataEntity = GetById(dataContext, entity.Id);
                        if (dataEntity.IsNull())
                        {
                            throw new ArgumentException("Cannot find entity '{0}' with id '{1}'".ParseTemplate(typeof(CompetitionType).Name, entity.Id));
                        }
                        var created = dataEntity.Created;
                        var rowStatus = dataEntity.RowStatus;
                        MapEntityToDataEntity(entity, dataEntity);
                        dataEntity.Created = created;
                        dataEntity.RowStatus = rowStatus;
                        dataEntity.Updated = DateTime.UtcNow;
                        dataContext.SubmitChanges();
                    });
        }
    }
}