using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.Utilities;

namespace Simple.SAMS.Competitions.Data
{
    public class CompetitionRepository : DataRepositoryBase<CompetitionsDataContext>, ICompetitionRepository
    {
        public void UpdateCompetitionPlayersPoints(int competitionId, UpdatePlayerPointsInfo[] players)
        {
            UseDataContext(dataContext =>
                               {
                                   var competitionPlayers =
                                       dataContext.CompetitionPlayers.Where(cp => cp.CompetitionId == competitionId);
                                   var competitionPlayersMap = new Dictionary<int, CompetitionPlayer>();
                                   competitionPlayers.ForEach(cp =>
                                                                  {
                                                                      competitionPlayersMap[cp.PlayerId] = cp;
                                                                  });
                                   foreach (var updatePlayerPointsInfo in players)
                                   {
                                       CompetitionPlayer player;
                                       if (competitionPlayersMap.TryGetValue(updatePlayerPointsInfo.PlayerId, out player))
                                       {
                                           if (updatePlayerPointsInfo.Points.HasValue)
                                           {
                                               player.Points = updatePlayerPointsInfo.Points.Value;
                                           }
                                           if (updatePlayerPointsInfo.Position.HasValue)
                                           {
                                               player.Position = updatePlayerPointsInfo.Position.Value;
                                           }
                                       }
                                   }

                                   dataContext.SubmitChanges();
                               });
        }

        private void MapCompetitionDataToHeader(Competition competition, CompetitionHeaderInfo competitionHeader)
        {
            competitionHeader.Id = competition.Id;
            competitionHeader.ReferenceId = competition.ReferenceId;
            competitionHeader.Name = competition.Name;
            competitionHeader.StartTime = competition.StartDate.ToLocalTime();
            if (competition.EndTime.HasValue)
            {
                competitionHeader.EndTime = competition.EndTime.Value.ToLocalTime();
            }
            competitionHeader.LastModified = competition.Updated.ToLocalTime();
            competitionHeader.Type = new Contracts.Competitions.CompetitionType()
                                         {
                                             Id = competition.TypeId,
                                             Name = competition.CompetitionType.Name,
                                             PlayersCount = competition.CompetitionType.PlayersCount
                                         };
            competitionHeader.Status = (CompetitionStatus)competition.Status;
        }

        private void MapCompetitionToData(CreateCompetitionInfo createCompetitionInfo, Competition competition)
        {
            competition.ReferenceId = createCompetitionInfo.ReferenceId;
            competition.Name = createCompetitionInfo.Name;
            competition.StartDate = createCompetitionInfo.StartTime;
            competition.TypeId = createCompetitionInfo.TypeId;
            competition.Site = createCompetitionInfo.Site;
            competition.SitePhone = createCompetitionInfo.SitePhone;
            competition.MainReferee = createCompetitionInfo.MainReferee;
            competition.MainRefereePhone = createCompetitionInfo.MainRefereePhone;
            competition.EndTime = createCompetitionInfo.EndTime;
        }



        public int Create(CreateCompetitionInfo createCompetitionInfo)
        {
            Requires.ArgumentNotNull(createCompetitionInfo, "headerInfo");
            Requires.IntArgumentPositive(createCompetitionInfo.TypeId, "headerInfo.TypeId");

            var result = default(int);

            UseDataContext(
                dataContext =>
                {
                    var competitionData = new Competition();
                    MapCompetitionToData(createCompetitionInfo, competitionData);
                    SetNewDataEntityCommonParameter(competitionData);
                    dataContext.Competitions.InsertOnSubmit(competitionData);
                    dataContext.SubmitChanges();

                    result = competitionData.Id;
                });
            return result;
        }

        private void SetNewDataEntityCommonParameter(Competition competitionData)
        {
            competitionData.Created = competitionData.Updated = DateTime.UtcNow;
            competitionData.RowStatus = 0;
        }

        public CompetitionHeaderInfo GetCompetition(int id)
        {
            var result = default(CompetitionHeaderInfo);

            UseDataContext(
                dataContext =>
                {
                    dataContext.ObjectTrackingEnabled = false;
                    dataContext.LoadOptions = GetCompetitionsLoadOptions();

                    var competitionData = dataContext.Competitions.FirstOrDefault(c => c.Id == id);
                    if (competitionData.IsNotNull())
                    {
                        result = new CompetitionHeaderInfo();
                        MapCompetitionDataToHeader(competitionData, result);
                    }
                });

            return result;
        }

        private DataLoadOptions GetCompetitionsLoadOptions()
        {
            var loadOptions = new DataLoadOptions();
            loadOptions.LoadWith<Competition>(c => c.CompetitionType);
            loadOptions.LoadWith<CompetitionPlayer>(cp => cp.Player);
            loadOptions.LoadWith<Match>(m => m.Player);
            loadOptions.LoadWith<Match>(m => m.Player5);
            loadOptions.LoadWith<Match>(m => m.Player6);
            loadOptions.LoadWith<Match>(m => m.Player7);
            loadOptions.LoadWith<Match>(m => m.MatchScores);
            return loadOptions;
        }

        public CompetitionDetails GetCompetitionUnplayedMatches(string competitionReferenceId)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionReferenceId, "competitionReferenceId");

            var result = GetCompetitionDetails(
                dataContext => dataContext.Competitions.FirstOrDefault(c => c.ReferenceId == competitionReferenceId),
                matches => matches.Where(m => !m.StartTime.HasValue && m.Status == (int)MatchStatus.PlayersAssigned));
            return result;
        }

        public Simple.SAMS.Contracts.Players.CompetitionPlayer GetCompetitionPlayer(int competitionId, int playerId)
        {
            var result = default(Simple.SAMS.Contracts.Players.CompetitionPlayer);
            UseDataContext(
                dataContext =>
                {
                    dataContext.ObjectTrackingEnabled = false;
                    dataContext.LoadOptions = GetCompetitionsLoadOptions();
                    var player =
                        dataContext.CompetitionPlayers.FirstOrDefault(
                            cp => cp.CompetitionId == competitionId && cp.Player.Id == playerId);

                    if (player.IsNotNull())
                    {
                        result = MapDataCompetitionPlayer(player, competitionId);
                    }
                });
            return result;
        }

        private CompetitionDetails GetCompetitionDetails(Func<CompetitionsDataContext, Competition> queryCompetitionDetails, Func<IQueryable<Match>, IQueryable<Match>> filterMatches = null)
        {
            var result = default(CompetitionDetails);

            UseDataContext(
                dataContext =>
                {
                    dataContext.ObjectTrackingEnabled = false;
                    dataContext.LoadOptions = GetCompetitionsLoadOptions();

                    var competitionData = queryCompetitionDetails(dataContext);
                    if (competitionData.IsNotNull())
                    {
                        result = new CompetitionDetails();
                        MapCompetitionDataToHeader(competitionData, result);

                        result.MainRefereeName = competitionData.MainReferee;
                        result.MainRefereePhone = competitionData.MainRefereePhone;
                        result.Site = competitionData.Site;
                        result.SitePhone = competitionData.SitePhone;

                        var players =
                            dataContext.CompetitionPlayers.Where(cp => cp.CompetitionId == competitionData.Id && cp.Player.RowStatus == 0).ToArray();
                        result.Players = players.Select(dataEntity => MapDataCompetitionPlayer(dataEntity, competitionData.Id)).ToArray();
                        var matches = dataContext.Matches.Where(m => m.CompetitionId == competitionData.Id && m.RowStatus == 0);

                        if (filterMatches.IsNotNull())
                        {
                            matches = filterMatches(matches);
                        }

                        result.Matches =
                            matches.Select(m => m.MapFromData()).ToArray();
                    }
                });

            return result;

        }

        private static Contracts.Players.CompetitionPlayer MapDataCompetitionPlayer(CompetitionPlayer dataEntity, int competitionId)
        {
            var entity = new Contracts.Players.CompetitionPlayer();
            AutoMapper.Mapper.DynamicMap(dataEntity.Player, entity);
            entity.CompetitionRank = dataEntity.Rank.GetValueOrDefault();
            entity.Replaceable =
                dataEntity.Player.Matches.Count(
                    m => m.CompetitionId == competitionId && m.Status >= (int)MatchStatus.Playing) == 0;
            entity.Source = (CompetitionPlayerSource)dataEntity.Source;
            entity.Section = (CompetitionSection)dataEntity.Section;
            entity.Status =
                (CompetitionPlayerStatus)dataEntity.Status.GetValueOrDefault((int)CompetitionPlayerStatus.Active);
            entity.Points = dataEntity.Points;
            entity.Position = dataEntity.Position;
            return entity;
        }

        public CompetitionDetails GetCompetitionDetails(int id)
        {
            var result = GetCompetitionDetails(dataContext => dataContext.Competitions.FirstOrDefault(c => c.Id == id));

            return result;
        }

        public CompetitionsSearchResult SearchCompetitions(CompetitionSearchQuery searchQuery)
        {
            var result = new CompetitionsSearchResult();
            var items = new List<CompetitionHeaderInfo>();

            UseDataContext(
                dataContext =>
                {
                    dataContext.ObjectTrackingEnabled = false;
                    dataContext.LoadOptions = GetCompetitionsLoadOptions();

                    var competitionQueryResult = dataContext.Competitions;
                    var pagedResults = competitionQueryResult.Skip(searchQuery.StartIndex).Take(searchQuery.PageSize);
                    result.TotalCount = competitionQueryResult.Count();
                    foreach (var dataItem in pagedResults)
                    {
                        var item = new CompetitionHeaderInfo();
                        MapCompetitionDataToHeader(dataItem, item);
                        items.Add(item);
                    }
                });

            result.Items = items.ToArray();

            return result;
        }

        public void AddPlayersToCompetition(int competitionId, PlayerInCompetition[] players)
        {
            UseDataContext(
                dataContext =>
                {
                    foreach (var playerInCompetition in players)
                    {
                        var playerAlreadyInCompetition =
                            dataContext.CompetitionPlayers.Any(
                                player =>
                                player.CompetitionId == competitionId &&
                                player.PlayerId == playerInCompetition.PlayerId);
                        if (!playerAlreadyInCompetition)
                        {
                            dataContext.CompetitionPlayers.InsertOnSubmit(
                                new CompetitionPlayer()
                                    {
                                        CompetitionId = competitionId,
                                        PlayerId = playerInCompetition.PlayerId,
                                        Rank = playerInCompetition.Rank,
                                        Source = (int)playerInCompetition.Source,
                                        Section = (int)playerInCompetition.Section,
                                        Status = (int)playerInCompetition.Status
                                    });
                        }

                        dataContext.SubmitChanges();
                    }

                });
        }

        public bool DoesCompetitionExists(int competitionId)
        {
            var exists = false;
            UseDataContext(
                dataContext =>
                {
                    exists = dataContext.Competitions.Any(competiton => competiton.Id == competitionId);
                });
            return exists;
        }


        public void UpdateCompetitionsByReferenceId(CreateCompetitionInfo[] competitions)
        {
            UseDataContext(
                dataContext =>
                {
                    competitions.ForEach(
                        competition =>
                        {
                            var dataCompetition = dataContext.Competitions.FirstOrDefault(dc => dc.ReferenceId == competition.ReferenceId);

                            if (dataCompetition.IsNull())
                            {
                                dataCompetition = new Competition();
                                SetNewDataEntityCommonParameter(dataCompetition);
                                dataContext.Competitions.InsertOnSubmit(dataCompetition);
                            }
                            else
                            {
                                dataCompetition.Updated = DateTime.UtcNow;
                            }

                            MapCompetitionToData(competition, dataCompetition);

                        });

                    dataContext.SubmitChanges();
                });
        }

        public void UpdateCompetitionStatus(int competitionId, CompetitionStatus newStatus)
        {
            UseDataContext(
                dataContext =>
                {
                    var dataCompetition = dataContext.Competitions.FirstOrDefault(dc => dc.Id == competitionId);

                    if (dataCompetition.IsNull())
                    {
                        throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
                    }

                    dataCompetition.Status = (int)newStatus;
                    dataCompetition.Updated = DateTime.UtcNow;

                    dataContext.SubmitChanges();
                });
        }

        public CompetitionHeaderInfo[] GetCompetitionsByStatus(CompetitionStatus status)
        {
            var competitions = new List<CompetitionHeaderInfo>();

            UseDataContext(
                dataContext =>
                {
                    var competitionsByStatus = dataContext.Competitions.Where(comp => comp.Status == (int)status);

                    competitionsByStatus.ForEach(dataCompetition =>
                                                     {
                                                         var competitionHeader =
                                                             new CompetitionHeaderInfo();
                                                         MapCompetitionDataToHeader(dataCompetition, competitionHeader);

                                                         competitions.Add(competitionHeader);
                                                     });
                });

            return competitions.ToArray();
        }

        public void RemovePlayerFromCompetition(int competitionId, int playerId, CompetitionPlayerStatus status, string reason)
        {
            UseDataContext(dataContext =>
                               {
                                   var relevantPlayers =
                                       dataContext.CompetitionPlayers.Where(
                                           cp => cp.CompetitionId == competitionId && cp.PlayerId == playerId);

                                   relevantPlayers.ForEach(
                                       player =>
                                       {
                                           player.Status = (int)status;
                                           player.Reason = reason;
                                       });
                                   dataContext.SubmitChanges();
                               });
        }


        public int? GetCompetitionIdByReferenceId(string referenceId)
        {
            var result = default(int?);
            UseDataContext(dataContext =>
                               {
                                   var competition = dataContext.Competitions.FirstOrDefault(c => c.ReferenceId == referenceId);
                                   if (competition.IsNotNull())
                                   {
                                       result = competition.Id;
                                   }
                               });
            return result;
        }
    }
}
