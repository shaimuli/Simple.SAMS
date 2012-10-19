using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;
using Simple.Utilities;

namespace Simple.SAMS.Competitions.Data
{
    public class CompetitionRepository : DataRepositoryBase<CompetitionsDataContext>, ICompetitionRepository
    {

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
            competitionHeader.Type = new EntityReference()
                                         {
                                             Id = competition.TypeId,
                                             Text = competition.CompetitionType.Name
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
                        var players =
                            dataContext.CompetitionPlayers.Where(cp => cp.CompetitionId == competitionData.Id && cp.Player.RowStatus == 0).Select(cp => cp.Player).ToArray();
                        result.Players = players.Select(dataEntity =>
                        {
                            var entity = new Contracts.Players.Player();
                            AutoMapper.Mapper.DynamicMap(dataEntity, entity);
                            return entity;
                        }).ToArray();
                        var matches = dataContext.Matches.Where(m => m.CompetitionId == competitionData.Id && m.RowStatus == 0);

                        if (filterMatches.IsNotNull())
                        {
                            matches = filterMatches(matches);
                        }

                        result.Matches =
                            matches.Select(m => CreateMatchFromData(m)).ToArray();
                    }
                });

            return result;

        }

        private MatchHeaderInfo CreateMatchFromData(Match match)
        {
            var result = new MatchHeaderInfo()
                       {
                           Id = match.Id,
                           StartTime = match.StartTime,
                           Status = (MatchStatus)match.Status,
                           Section = (CompetitionSection)match.SectionId,
                           StartTimeType = (StartTimeType)match.StartTimeType,
                           Round = match.Round,
                           Position = match.Position
                       };
            var matchPlayers = match.MatchPlayers.Select(mp => new { mp.Player, mp.Position }).ToArray();
            var player1 = matchPlayers.FirstOrDefault(mp => mp.Position == 1);
            if (player1 != null)
            {
                result.Player1 = CreateMatchPlayerFromData(player1.Player);
            }
            var player2 = matchPlayers.FirstOrDefault(mp => mp.Position == 2);
            if (player2 != null)
            {
                result.Player2 = CreateMatchPlayerFromData(player2.Player);
            }
            var player3 = matchPlayers.FirstOrDefault(mp => mp.Position == 3);
            if (player3 != null)
            {
                result.Player3 = CreateMatchPlayerFromData(player3.Player);
            }
            var player4 = matchPlayers.FirstOrDefault(mp => mp.Position == 4);
            if (player4 != null)
            {
                result.Player4 = CreateMatchPlayerFromData(player4.Player);
            }

            return result;
        }

        private Contracts.Competitions.MatchPlayer CreateMatchPlayerFromData(Player player)
        {
            var result = new Contracts.Competitions.MatchPlayer();

            result.Id = player.Id;
            result.IdNumber = player.IdNumber;

            result.LocalFirstName = player.LocalFirstName;
            result.LocalLastName = player.LocalLastName;
            result.EnglishFirstName = player.EnglishFirstName;
            result.EnglishLastName = player.EnglishLastName;
            return result;
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
                            dataContext.CompetitionPlayers.InsertOnSubmit(new CompetitionPlayer() { CompetitionId = competitionId, PlayerId = playerInCompetition.PlayerId, Rank = playerInCompetition.Rank });
                        }
                    }
                    dataContext.SubmitChanges();
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

    }
}
