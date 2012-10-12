﻿using System;
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
            competitionHeader.LastModified = competition.Updated.ToLocalTime();
            competitionHeader.Type = new EntityReference() { Id = competition.TypeId, Text = competition.CompetitionType.Name };
            competitionHeader.Status = (CompetitionStatus) competition.Status;
        }

        private void MapCompetitionToData(CompetitionHeaderInfo headerInfo, Competition competition)
        {
            if (headerInfo.Id.HasValue)
            {
                competition.Id = headerInfo.Id.Value;
            }
            competition.ReferenceId = headerInfo.ReferenceId;
            competition.Name = headerInfo.Name;
            competition.StartDate = headerInfo.StartTime;
            competition.TypeId = headerInfo.Type.Id;

        }

        public int Create(CompetitionHeaderInfo headerInfo)
        {
            Requires.ArgumentNotNull(headerInfo, "headerInfo");
            Requires.ArgumentNotNull(headerInfo.Type, "headerInfo.Type");
            if (headerInfo.Id.HasValue)
            {
                throw new ArgumentException("headerInfo.Id must be null for new competition.");
            }
            var result = default(int);

            UseDataContext(
                dataContext =>
                {
                    var competitionData = new Competition();
                    MapCompetitionToData(headerInfo, competitionData);
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

        public CompetitionDetails GetCompetitionDetails(int id)
        {
            var result = default(CompetitionDetails);

            UseDataContext(
                dataContext =>
                {
                    dataContext.ObjectTrackingEnabled = false;
                    dataContext.LoadOptions = GetCompetitionsLoadOptions();

                    var competitionData = dataContext.Competitions.FirstOrDefault(c => c.Id == id);
                    if (competitionData.IsNotNull())
                    {
                        result = new CompetitionDetails();
                        MapCompetitionDataToHeader(competitionData, result);

                        var matches = dataContext.Matches.Where(m => m.CompetitionId == id && m.RowStatus == 0);
                        result.Matches =
                            matches.Select(m => new MatchHeaderInfo()
                                                    {
                                                        Id = m.Id,
                                                        StartTime = m.StartTime,
                                                        Status = (MatchStatus)m.Status,
                                                        Round =m.Round,
                                                        Position = m.Position,
                                                        Player1 = new Contracts.Competitions.MatchPlayer()
                                                                      {
                                                                          PlayerId = 1,
                                                                          PlayerName = "Not loaded #1"
                                                                      },
                                                        Player2 = new Contracts.Competitions.MatchPlayer()
                                                                      {
                                                                          PlayerId = 2,
                                                                          PlayerName = "Not loaded #2"
                                                                      }
                                                    }).ToArray();
                    }
                });

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
                                dataContext.CompetitionPlayers.InsertOnSubmit(new CompetitionPlayer(){CompetitionId = competitionId, PlayerId = playerInCompetition.PlayerId, Rank = playerInCompetition.Rank });
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


        public void UpdateCompetitionsByReferenceId(CompetitionHeaderInfo[] competitions)
        {
            UseDataContext(
                dataContext =>
                    {
                        competitions.ForEach(
                            competition =>
                                {
                                    var dataCompetition = dataContext.Competitions.FirstOrDefault(dc=>dc.ReferenceId == competition.ReferenceId);

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

                        dataCompetition.Status = (int) newStatus;
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
                        var competitionsByStatus = dataContext.Competitions.Where(comp => comp.Status == (int) status);

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
