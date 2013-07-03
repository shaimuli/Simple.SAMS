using System.Collections.Generic;
using System.Linq;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Competitions.Data
{
    public class PlayerRepository : EntityRepositoryBase<Contracts.Players.Player, Player, CompetitionsDataContext>, IPlayersRepository
    {
        protected override Player GetById(CompetitionsDataContext dataContext, int id)
        {
            return dataContext.Players.FirstOrDefault(p => p.Id == id);
        }

        protected override Contracts.Players.Player CreateEntity(Player dataEntity)
        {
            return new Contracts.Players.Player();
        }

        protected override void MapDataEntityToEntity(Player dataEntity, Contracts.Players.Player entity)
        {
            AutoMapper.Mapper.DynamicMap(dataEntity, entity);
        }

        protected override Player CreateDataEntity(Contracts.Players.Player entity)
        {
            return new Player();
        }

        protected override void MapEntityToDataEntity(Contracts.Players.Player entity, Player dataEntity)
        {
            AutoMapper.Mapper.DynamicMap(entity, dataEntity);
        }

        protected override void InsertEntity(CompetitionsDataContext dataContext, Player dataEntity)
        {
            dataContext.Players.InsertOnSubmit(dataEntity);
        }

        public class PlayerByIdNumberEqualityComparer : IEqualityComparer<Contracts.Players.Player>
        {
            public bool Equals(Contracts.Players.Player x, Contracts.Players.Player y)
            {
                return x.IdNumber == y.IdNumber;
            }

            public int GetHashCode(Contracts.Players.Player obj)
            {
                return obj.IdNumber.GetHashCode();
            }
        }

        public Dictionary<string, int> MatchPlayerByIdNumber(Contracts.Players.Player[] players)
        {
            var matchedPlayers = new Dictionary<string, int>();

            UseDataContext(
                dataContext =>
                    {
                        var dataPlayers = new List<Player>();
                        foreach (var player in players.Distinct(new PlayerByIdNumberEqualityComparer()))
                        {
                            var dataPlayer = dataContext.Players.FirstOrDefault(p => p.IdNumber == player.IdNumber);
                            if (dataPlayer.IsNull())
                            {
                                dataPlayer = CreateDataEntity(player);
                                MapEntityToDataEntity(player, dataPlayer);
                                SetNewDataEntityParameters(dataPlayer);
                                
                                dataContext.Players.InsertOnSubmit(dataPlayer);
                            }
                            dataPlayers.Add(dataPlayer);
                        }
                        
                        dataContext.SubmitChanges();

                        dataPlayers.ForEach(dp =>
                                                {
                                                    matchedPlayers[dp.IdNumber] = dp.Id;
                                                });
                    });

            return matchedPlayers;
        }


        public int? GetPlayerIdByIdNumber(string idNumber)
        {
            var result = default(int?);

            UseDataContext(dataContext =>
                               {
                                   var player = dataContext.Players.FirstOrDefault(p => p.IdNumber == idNumber);
                                   if (player.IsNotNull())
                                   {
                                       result = player.Id;
                                   }
                               });

            return result;
        }
    }
}