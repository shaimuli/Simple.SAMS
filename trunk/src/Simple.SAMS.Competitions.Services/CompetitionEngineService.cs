using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using FileHelpers;
using Rhino.Etl.Core.Files;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;
using Simple.SAMS.Utilities;
using Simple.SimplyLog.ImportExport;

namespace Simple.SAMS.Competitions.Services
{
    public class CompetitionEngineService : ICompetitionsEngine
    {

        public void QualifyMatchWinner(int matchId)
        {
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            var match = competitionMatchesRepository.GetMatch(matchId);

            if (match.StartTime.HasValue &&
                match.Result.HasValue && match.Result.Value != MatchResult.Pause &&
                match.Winner != MatchWinner.None &&
                (match.Player1 != null || match.Player2 != null))
            {
                if (match.IsFinal)
                {
                    if (match.Section == CompetitionSection.Qualifying)
                    {
                        QualifyToFinal(match);
                    }
                }
                else
                {
                    QualifySameSection(match);
                }
            }

        }

        private void QualifyToThirdPlace(MatchHeaderInfo match)
        {
            var positioningEngine = GetPositioningEngine(CompetitionMethod.Knockout);
            var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();

            var competitionDetails = competitionRepository.GetCompetitionDetails(match.CompetitionId);
            var looserId = (match.Winner == MatchWinner.Player1 ? match.Player2 : match.Player1).Id;
            var thirdPlaceMatch = competitionDetails.Matches.Last();
            var updateInfo = new UpdatePlayerPositionInfo()
                                 {
                                     PlayerId = looserId,
                                     Position = thirdPlaceMatch.Player1.IsNull() ? 0 : 1,
                                     MatchId = thirdPlaceMatch.Id
                                 };
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            competitionMatchesRepository.UpdatePlayersPosition(match.CompetitionId, new[] { updateInfo });

        }

        private void QualifyToFinal(MatchHeaderInfo match)
        {
            var winner = (match.Winner == MatchWinner.Player1 ? match.Player1 : match.Player2);
            if (winner != null)
            {
                var winnerId = winner.Id;

                PositionPlayerInSection(match.CompetitionId, winnerId, CompetitionSection.Final);
            }
        }

        public void PositionPlayerInSection(int competitionId, int playerId, CompetitionSection section)
        {
            var positioningEngine = GetPositioningEngine(CompetitionMethod.Knockout);
            var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();

            var competitionDetails = competitionRepository.GetCompetitionDetails(competitionId);


            var updatePlayerInfo = positioningEngine.AddPlayerToSection(playerId, section, competitionDetails);
            if (updatePlayerInfo.IsNotNull())
            {
                var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
                competitionMatchesRepository.UpdatePlayersPosition(competitionId, new[] {updatePlayerInfo});
            }
        }

        private void QualifySameSection(MatchHeaderInfo match)
        {
            var competitionId = match.CompetitionId;
            var winner = match.Winner == MatchWinner.Player1 ? match.Player1 : match.Player2;
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            var qualifyToMatch = competitionMatchesRepository.GetMatchByRelativePosition(competitionId, match.Section, match.Round + 1,
                                                                                         match.RoundRelativePosition / 2);
            if (qualifyToMatch.IsNotNull())
            {
                competitionMatchesRepository.UpdatePlayersPosition(competitionId,
                                                                   new[]
                                                                       {
                                                                           new UpdatePlayerPositionInfo
                                                                               {
                                                                                   MatchId = qualifyToMatch.Id,
                                                                                   PlayerId = winner.Id,
                                                                                   Position =
                                                                                       match.RoundRelativePosition%2 == 0
                                                                                           ? 0
                                                                                           : 1
                                                                               }
                                                                       });
            }

            if (match.IsSemiFinal)
            {
                QualifyToThirdPlace(match);
            }

        }

        public void CreateCompetitionsMatches(CompetitionHeaderInfo[] competitions)
        {
            var matchProvisioningEngineFactory = ServiceProvider.Get<IMatchProvisioningEngineFactory>();
            var matchesCache = new Dictionary<int, MatchHeaderInfo[]>();
            var competitionTypeRepository = ServiceProvider.Get<ICompetitionTypeRepository>();


            foreach (var competition in competitions)
            {
                var competitionTypeId = competition.Type.Id;
                MatchHeaderInfo[] matches;
                if (!matchesCache.TryGetValue(competitionTypeId, out matches))
                {
                    var competitionType = competitionTypeRepository.Get(competitionTypeId);
                    var matchProvisioningEngine = matchProvisioningEngineFactory.Create(competitionType);
                    matches = matchProvisioningEngine.BuildMatches(competitionType);
                    matchesCache[competitionTypeId] = matches;
                }

                var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
                competitionMatchesRepository.AddCompetitionMatches(competition.Id.Value, matches.Select(m => m.CloneDataContract()).ToArray());
            }

        }

        public void AddPlayersToCompetition(int competitionId, AddCompetitionPlayerInfo[] players)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            var competition = competitionsRepository.GetCompetitionDetails(competitionId);
            if (competition.IsNull())
            {
                throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
            }
            var competitionType = competitionTypesRepository.Get(competition.Type.Id);
            if (competition.Players.Length == competitionType.PlayersCount)
            {
                throw new ArgumentException("Competition '{0}' is already full.".ParseTemplate(competitionId));
            }
            var qualifyingPlayersCount = players.Count(p => p.Section == CompetitionSection.Qualifying);
            if (qualifyingPlayersCount > 0)
            {
                if (competitionType.QualifyingPlayersCount == 0)
                {
                    throw new ArgumentException(
                        "Competition '{0}' does not have qualifying section.".ParseTemplate(competitionId));
                }
                if (qualifyingPlayersCount > competitionType.QualifyingPlayersCount)
                {
                    throw new ArgumentException(
                        "Competition '{0}' has only {1} players in qualifying section but more players designated to that section.".ParseTemplate(competitionId, competitionType.QualifyingPlayersCount));
                }
                var competitionQualifyingPlayersCount =
                    competition.Players.Count(p => p.Section == CompetitionSection.Qualifying);
                if (competitionQualifyingPlayersCount == competitionType.QualifyingPlayersCount)
                {
                    throw new ArgumentException(
                        "Competition '{0}' qualifying section is already full.".ParseTemplate(competitionId));
                }

            }

            var competitionPlayers = new List<PlayerInCompetition>();
            var finalPlayersCount = competitionType.PlayersCount - competitionType.QualifyingToFinalPlayersCount;
            for (var i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var rank = competitionType.RankPlayer(player.Player);
                var section = player.Section ?? CompetitionSection.Final;

                var playerInCompetition = new PlayerInCompetition()
                                              {
                                                  PlayerId = player.Player.Id,
                                                  Rank = rank,
                                                  Source = player.Source,
                                                  Section = section
                                              };
                competitionPlayers.Add(playerInCompetition);
            }

            var playersToTake = Math.Min(players.Length - finalPlayersCount, competitionType.QualifyingPlayersCount);
            if (competitionType.QualifyingPlayersCount > 0 && playersToTake > competitionType.QualifyingToFinalPlayersCount)
            {
                
                var qualifyingPlayers =
                    competitionPlayers.OrderByDescending(p => p.Rank).Take(playersToTake - qualifyingPlayersCount);
                qualifyingPlayers.ForEach(cp => cp.Section = CompetitionSection.Qualifying);
            }

            competitionsRepository.AddPlayersToCompetition(competitionId, competitionPlayers.ToArray());
        }

        public CompetitionPlayer[] GetCompetitionPlayers(string playersFileUrl)
        {
            var fileName = DownloadFile(playersFileUrl);
            var players = LoadPlayersFromFile(fileName);

            return players.Select(p => new CompetitionPlayer
                                           {
                                               IdNumber = p.IdNumber,
                                               LocalFirstName = p.LocalFirstName,
                                               LocalLastName = p.LocalLastName,
                                               EnglishFirstName = p.EnglishFirstName,
                                               EnglishLastName = p.EnglishLastName,
                                               BirthDate = p.BirthDate,
                                               IsFemale = p.IsFemale.GetValueOrDefault(),
                                               Phone = p.Phone,
                                               IPIN = p.IPIN,
                                               Country = p.Country,
                                               NationalRank = p.NationalRank,
                                               EuropeInternationalRank = p.EuropeInternationalRank,
                                               YouthInternationalRank = p.YouthInternationalRank,
                                               Source = p.Source == "WC" ? CompetitionPlayerSource.Wildcard : CompetitionPlayerSource.Regular,
                                               AverageScore = p.AverageScore,
                                               AccumulatedScore = p.AccumulatedScore,
                                               CompetitionReferenceId = p.CompetitionReferenceId
                                           }).ToArray();
        }

        private string DownloadFile(string fileUrl)
        {
            var webClient = new WebClient();
            var fileName = Path.GetTempFileName();
            webClient.DownloadFile(fileUrl, fileName);
            return fileName;
        }

        public int CreateCompetition(CreateCompetitionInfo competitionCreateInfo)
        {
            var repository = ServiceProvider.Get<ICompetitionRepository>();
            competitionCreateInfo.ReferenceId = Guid.NewGuid().ToString();
            var result = repository.Create(competitionCreateInfo);

            return result;
        }

        private IEnumerable<T> LoadRecords<T>(string fileName)
            where T: new()
        {
            var records = new List<T>();
            using (var dataTable = new DataTable())
            {
                ExcelDocumentHelper.LoadFromSheet(fileName, dataTable, inferColumns: true);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var record = new T();
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        var value = dataRow[dataColumn.ColumnName];
                        if (value != null && !Convert.IsDBNull(value))
                        {

                            var propertyInfo = dataColumn.ExtendedProperties["Property"] as PropertyInfo;
                            if (propertyInfo == null)
                            {
                                propertyInfo = typeof(T).GetProperty(dataColumn.ColumnName);
                                dataColumn.ExtendedProperties["Property"] = propertyInfo;
                            }
                            if (propertyInfo == null)
                            {
                                throw new ArgumentException("Column '{0}' is invalid".ParseTemplate(dataColumn.ColumnName));
                            }
                            if (propertyInfo.PropertyType == typeof(DateTime?) ||
                                propertyInfo.PropertyType == typeof(DateTime))
                            {
                                value = DateTime.FromOADate(double.Parse(value.ToString()));
                            }
                            else if (propertyInfo.PropertyType == typeof(bool?) ||
                                     propertyInfo.PropertyType == typeof(bool))
                            {
                                value = "true,TRUE,1".Contains(value.ToString());
                            }
                            else if (propertyInfo.PropertyType == typeof(int?) ||
                                     propertyInfo.PropertyType == typeof(int))
                            {
                                value = int.Parse(value.ToString());
                            }
                            else
                            {
                                value = Convert.ChangeType(value, propertyInfo.PropertyType);
                            }
                            propertyInfo.SetValue(record, value);
                        }
                    }
                    records.Add(record);
                }
            }
            return records.ToArray();        
        }

        private IEnumerable<PlayerRecord> LoadPlayersFromFile(string fileName)
        {
            return LoadRecords<PlayerRecord>(fileName);
        }
        private IEnumerable<CompetitionRecord> LoadCompetitionsFromFile(string fileName)
        {
            return LoadRecords<CompetitionRecord>(fileName);
        }

        private class CompetitionRecord
        {
            public string ReferenceId { get; set; }
            public string Name { get; set; }
            public int Type { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public string Site { get; set; }
            public string SitePhone { get; set; }
            public string MainReferee { get; set; }
            public string MainRefereePhone { get; set; }
        }

        private class PlayerRecord
        {
            public string IdNumber { get; set; }
            public string LocalFirstName { get; set; }
            public string LocalLastName { get; set; }
            public string EnglishFirstName { get; set; }
            public string EnglishLastName { get; set; }
            public string Phone { get; set; }
            public DateTime? BirthDate { get; set; }
            public bool? IsFemale { get; set; }

            public int? NationalRank { get; set; }

            public int? EuropeInternationalRank { get; set; }

            public int? YouthInternationalRank { get; set; }
            public string IPIN { get; set; }
            public string Country { get; set; }
            public string Source { get; set; }
            public int? AverageScore { get; set; }
            public int? AccumulatedScore { get; set; }
            public string CompetitionReferenceId { get; set; }
        }



        public CreateCompetitionInfo[] GetCompetitions(string competitionsFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionsFileUrl, "competitionsFileUrl");
            var fileName = DownloadFile(competitionsFileUrl);
            var competitionRecords = LoadCompetitionsFromFile(fileName);
            return competitionRecords.Select(cr => new CreateCompetitionInfo()
            {
                Name = cr.Name,
                StartTime = cr.StartTime,
                TypeId = MapCompetitionTypeId(cr.Type),
                ReferenceId = cr.ReferenceId,
                EndTime = cr.EndTime,
                Site = cr.Site,
                SitePhone = cr.SitePhone,
                MainReferee = cr.MainReferee,
                MainRefereePhone = cr.MainRefereePhone
            }).ToArray();
        }

        private int MapCompetitionTypeId(int competitionRecordTypeId)
        {
            return competitionRecordTypeId;
        }


        public void ImportCompetitions(CreateCompetitionInfo[] competitions)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            competitionsRepository.UpdateCompetitionsByReferenceId(competitions);
        }


        public void UpdatePlayersPosition(int competitionId, CompetitionSection section)
        {
            var competitionDetails = GetCompetitionDetails(competitionId);

            var positioningEngine = GetPositioningEngine(competitionDetails.Type.Method);

            var positions = positioningEngine.PositionPlayers(competitionDetails, section);

            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();

            competitionMatchesRepository.UpdatePlayersPosition(competitionId, positions);
        }

        private static IPositioningEngine GetPositioningEngine(CompetitionMethod competitionMethod)
        {
            var positioningEngineFactory = ServiceProvider.Get<IPositioningEngineFactory>();
            var positioningEngine = positioningEngineFactory.Create(competitionMethod);
            if (positioningEngine.IsNull())
            {
                throw new ApplicationException(
                    "Positioning engine factory '{0}' returned null, instance of {1} is expected.".ParseTemplate(
                        positioningEngineFactory.GetType().FullName, typeof(IPositioningEngine).FullName));
            }
            return positioningEngine;
        }

        public CompetitionDetails GetCompetitionDetails(int competitionId)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var competitionsTypeRepository = ServiceProvider.Get<ICompetitionTypeRepository>();

            var competitionDetails = competitionsRepository.GetCompetitionDetails(competitionId);
            var competitionType = competitionsTypeRepository.Get(competitionDetails.Type.Id);
            competitionDetails.Type = competitionType;
            if (competitionType.QualifyingPlayersCount > 0)
            {
                var actualQualifyingPlayersCount =
                    competitionDetails.Players.Count(p => p.Section == CompetitionSection.Qualifying);
                competitionDetails.CanAddToQualifying =
                    actualQualifyingPlayersCount <
                    competitionType.QualifyingPlayersCount;
            }

            var actualFinalPlayers = competitionDetails.Players.Count(p => p.Section == CompetitionSection.Final);
            var spotsInFinal = competitionType.PlayersCount - actualFinalPlayers;
            if (competitionType.QualifyingPlayersCount > 0)
            {
                spotsInFinal -= competitionType.QualifyingToFinalPlayersCount;
            }

            competitionDetails.CanAddToFinal = spotsInFinal > 0;

            return competitionDetails;
        }

        public void UpdateMatchResult(MatchScoreUpdateInfo scoreUpdateInfo)
        {
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            competitionMatchesRepository.UpdateMatchResult(scoreUpdateInfo);
        }
        public void UpdateMatchScore(MatchScoreUpdateInfo scoreUpdateInfo)
        {
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            competitionMatchesRepository.UpdateMatchScore(scoreUpdateInfo);
        }

        public void UpdateMatchStartTime(MatchStartTimeUpdateInfo startTimeUpdateInfo)
        {
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            competitionMatchesRepository.UpdateMatchStartTime(startTimeUpdateInfo);
        }


        public void RemovePlayerFromUnplayedMatches(int competitionId, int playerId)
        {
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            competitionMatchesRepository.RemovePlayerFromUnplayedMatches(competitionId, playerId);
        }

        public void RemovePlayerFromCompetition(int competitionId, int playerId, CompetitionPlayerStatus status, string reason)
        {
            var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();
            competitionRepository.RemovePlayerFromCompetition(competitionId, playerId, status, reason);
        }


        public bool AreAllValidCompetitionTypes(int[] competitionTypes)
        {
            var competitionTypeRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            return competitionTypeRepository.AreCompetitionTypesExist(competitionTypes);
        }

        public void QualifyByeMatches(int id, CompetitionSection section)
        {
            
        }
    }
}
