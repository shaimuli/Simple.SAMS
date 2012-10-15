using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Competitions.Data
{
    public class CompetitionTypeRepository : EntityRepositoryBase<Contracts.Competitions.CompetitionType, CompetitionType, CompetitionsDataContext>, ICompetitionTypeRepository
    {
        private const int MaxResults = 100;
        public Contracts.Competitions.CompetitionType[] GetCompetitionTypes()
        {
            var result = new List<Contracts.Competitions.CompetitionType>();

            UseDataContext(
                dataContext =>
                {
                    var competitionTypes = dataContext.CompetitionTypes.Take(MaxResults);
                    result.AddRange(competitionTypes.Select(type => new Contracts.Competitions.CompetitionType() { Id = type.Id, Name = type.Name }));
                });

            return result.ToArray();
        }

        protected override CompetitionType GetById(CompetitionsDataContext dataContext, int id)
        {
            return dataContext.CompetitionTypes.FirstOrDefault(ct=>ct.Id == id);
        }

        protected override Contracts.Competitions.CompetitionType CreateEntity(CompetitionType dataEntity)
        {
            return new Contracts.Competitions.CompetitionType();
        }

        protected override void MapDataEntityToEntity(CompetitionType dataEntity, Contracts.Competitions.CompetitionType entity)
        {
            entity.Name = dataEntity.Name;
            entity.Method = (CompetitionMethod) dataEntity.CompetitionMethod;
            entity.PlayersCount = dataEntity.PlayersCount;
            entity.QualifyingPlayersCount = dataEntity.QualifyingPlayersCount;
            entity.WildcardPlayersCount = dataEntity.WildcardPlayersCount;
            entity.PairsCount = dataEntity.PairsCount;
            entity.QualifyingPairsCount = dataEntity.QualifyingPairsCount;
            entity.WildcardPairsCount = dataEntity.WildcardPairsCount;
            entity.HasConsolation = dataEntity.HasConsolation;
        }

        protected override CompetitionType CreateDataEntity(Contracts.Competitions.CompetitionType entity)
        {
            return new CompetitionType();
        }

        protected override void MapEntityToDataEntity(Contracts.Competitions.CompetitionType entity, CompetitionType dataEntity)
        {
            dataEntity.Name = entity.Name;
            dataEntity.CompetitionMethod = (int) entity.Method;
            dataEntity.PlayersCount = entity.PlayersCount;
            dataEntity.QualifyingPlayersCount = entity.QualifyingPlayersCount;
            dataEntity.WildcardPlayersCount = entity.WildcardPlayersCount;
            dataEntity.PairsCount = entity.PairsCount;
            dataEntity.QualifyingPairsCount = entity.QualifyingPairsCount;
            dataEntity.WildcardPairsCount = entity.WildcardPairsCount;
            dataEntity.HasConsolation = entity.HasConsolation;
        }

        protected override void InsertEntity(CompetitionsDataContext dataContext, CompetitionType dataEntity)
        {
            dataContext.CompetitionTypes.InsertOnSubmit(dataEntity);
        }
    }
}
