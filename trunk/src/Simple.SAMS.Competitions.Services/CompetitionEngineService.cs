﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FileHelpers;
using Rhino.Etl.Core.Files;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;
using Simple.SAMS.Utilities;

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
                match.Player1 != null && match.Player2 != null)
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
            var winnerId = (match.Winner == MatchWinner.Player1 ? match.Player1 : match.Player2).Id;

            PositionPlayerInSection(match.CompetitionId, winnerId, CompetitionSection.Final);
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
                                               AccumulatedScore = p.AccumulatedScore
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

        private IEnumerable<PlayerRecord> LoadPlayersFromFile(string fileName)
        {
            var engine = FluentFile.For<PlayerRecord>();
            
            using (var file = engine.From(fileName))
            {
                foreach (var player in file.Cast<PlayerRecord>().Where(p=>p.LocalFirstName.NotNullOrEmpty() && p.EnglishFirstName.NotNullOrEmpty() && p.IdNumber.NotNullOrEmpty()))
                {
                    yield return player;
                }
            }
        }
        private IEnumerable<CompetitionRecord> LoadCompetitionsFromFile(string fileName)
        {
            var engine = FluentFile.For<CompetitionRecord>();

            using (var file = engine.From(fileName))
            {

                foreach (var player in file.Cast<CompetitionRecord>().Where(c=>c.ReferenceId.NotNullOrEmpty() && c.Name.NotNullOrEmpty()))
                {
                    yield return player;
                }
            }
        }

        [DelimitedRecord(","), IgnoreFirst]
        private class CompetitionRecord
        {
            public string ReferenceId;
            public string Name;
            public int TypeId;
            [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
            public DateTime StartTime;
            [FieldOptional] 
            [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
            public DateTime? EndTime;
            [FieldOptional]
            public string Site;
            
            [FieldOptional]
            public string SitePhone;

            [FieldOptional]
            public string MainReferee;
            [FieldOptional] 
            public string MainRefereePhone;
        }

        [DelimitedRecord(","), IgnoreFirst]
        private class PlayerRecord
        {
            public string IdNumber;
            public string LocalFirstName;
            [FieldOptional]
            public string LocalLastName;
            [FieldOptional]
            public string EnglishFirstName;
            [FieldOptional]
            public string EnglishLastName;
            [FieldOptional]
            public string Phone;
            [FieldOptional] 
            [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
            public DateTime? BirthDate;
            [FieldOptional]
            public bool? IsFemale;
            [FieldOptional]
            public int? NationalRank;
            [FieldOptional]
            public int? EuropeInternationalRank;
            [FieldOptional] 
            public int? YouthInternationalRank;
            [FieldOptional]
            public string IPIN;
            [FieldOptional] 
            public string Country;
            [FieldOptional()] 
            public string Source;
            [FieldOptional] 
            public int? AverageScore;

            [FieldOptional] public int? AccumulatedScore;
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
                TypeId = MapCompetitionTypeId(cr.TypeId),
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

        public void RemovePlayerFromCompetition(int competitionId, int playerId)
        {
            var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();
            competitionRepository.RemovePlayerFromCompetition(competitionId, playerId);
        }


    }
}
