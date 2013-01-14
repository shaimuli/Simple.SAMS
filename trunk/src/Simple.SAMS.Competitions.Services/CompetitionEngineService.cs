using System;
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
                                     Position = thirdPlaceMatch.Player1.IsNull() ?0:1,
                                     MatchId = thirdPlaceMatch.Id
                                 };
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            competitionMatchesRepository.UpdatePlayersPosition(match.CompetitionId, new[] { updateInfo });

        }

        private void QualifyToFinal(MatchHeaderInfo match)
        {
            var positioningEngine = GetPositioningEngine(CompetitionMethod.Knockout);
            var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();
            
            var competitionDetails = competitionRepository.GetCompetitionDetails(match.CompetitionId);
            var winnerId = (match.Winner == MatchWinner.Player1 ? match.Player1 : match.Player2).Id;
            var updatePlayerInfo = positioningEngine.AddPlayerToSection(winnerId, CompetitionSection.Final, competitionDetails);
            if (updatePlayerInfo.IsNotNull())
            {
                var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
                competitionMatchesRepository.UpdatePlayersPosition(match.CompetitionId, new[] {updatePlayerInfo});
            }
        }

        private  void QualifySameSection(MatchHeaderInfo match)
        {
            var competitionId = match.CompetitionId;
            var winner = match.Winner == MatchWinner.Player1 ? match.Player1 : match.Player2;
            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            var qualifyToMatch = competitionMatchesRepository.GetMatchByRelativePosition(competitionId, match.Section, match.Round + 1,
                                                                                         match.RoundRelativePosition/2);
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
            var competition = competitionsRepository.GetCompetition(competitionId);
            if (competition.IsNull())
            {
                throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
            }
            var competitionType = competitionTypesRepository.Get(competition.Type.Id);
            var competitionPlayers = new List<PlayerInCompetition>();
            for (var i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var rank = competitionType.RankPlayer(player.Player);
                var playerInCompetition = new PlayerInCompetition()
                                              {
                                                  PlayerId = player.Player.Id, 
                                                  Rank = rank, 
                                                  Source = player.Source,
                                                  Section = CompetitionSection.Final
                                              };
                competitionPlayers.Add(playerInCompetition);
            }

            if (competitionType.QualifyingPlayersCount > 0)
            {
                var qualifyingPlayers =
                    competitionPlayers.OrderByDescending(p => p.Rank).Take(competitionType.QualifyingPlayersCount);
                qualifyingPlayers.ForEach(cp=>cp.Section = CompetitionSection.Qualifying);
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
                                               IsFemale = p.IsFemale,
                                               Phone = p.Phone,
                                               IPIN = p.IPIN,
                                               Country = p.Country,
                                               NationalRank = p.NationalRank,
                                               EuropeInternationalRank = p.EuropeInternationalRank,
                                               YouthInternationalRank = p.YouthInternationalRank,
                                               Source = p.Source == "WC" ? CompetitionPlayerSource.Wildcard : CompetitionPlayerSource.Regular
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
            using (var file = FluentFile.For<PlayerRecord>().From(fileName))
            {
                foreach (var player in file.Cast<PlayerRecord>())
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

                foreach (var player in file.Cast<CompetitionRecord>())
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
            [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
            public DateTime? EndTime;
            public string Site;
            public string SitePhone;
            public string MainReferee;
            public string MainRefereePhone;
        }

        [DelimitedRecord(","), IgnoreFirst]
        private class PlayerRecord
        {
            public string IdNumber;
            public string LocalFirstName;
            public string LocalLastName;
            public string EnglishFirstName;
            public string EnglishLastName;
            public string Phone;
            [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
            public DateTime? BirthDate;
            public bool IsFemale;
            public int? NationalRank;
            public int? EuropeInternationalRank;
            public int? YouthInternationalRank;
            public string IPIN;
            public string Country;
            public string Source { get; set; }
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


        private void UpdateCompetitionPlayersPosition(int competitionId)
        {
            var competitionDetails = GetCompetitionDetails(competitionId);

            var positioningEngine = GetPositioningEngine(competitionDetails.Type.Method);

            var positions = positioningEngine.PositionPlayers(competitionDetails);

            var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
            
            competitionMatchesRepository.UpdatePlayersPosition(competitionId, positions);

            
            // TRACE
            //var json = positions.ToJson();
            //var file = "E:\\temp\\positions.json.txt";
            //if (File.Exists(file))
            //{
            //    File.Delete(file);
            //}
            //File.WriteAllText(file, json);
        }

        private static IPositioningEngine GetPositioningEngine(CompetitionMethod competitionMethod)
        {
            var positioningEngineFactory = ServiceProvider.Get<IPositioningEngineFactory>();
            var positioningEngine = positioningEngineFactory.Create(competitionMethod);
            if (positioningEngine.IsNull())
            {
                throw new ApplicationException(
                    "Positioning engine factory '{0}' returned null, instance of {1} is expected.".ParseTemplate(
                        positioningEngineFactory.GetType().FullName, typeof (IPositioningEngine).FullName));
            }
            return positioningEngine;
        }

        public CompetitionDetails GetCompetitionDetails(int competitionId)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var competitionsTypeRepository = ServiceProvider.Get<ICompetitionTypeRepository>();

            var competitionDetails = competitionsRepository.GetCompetitionDetails(competitionId);
            competitionDetails.Type = competitionsTypeRepository.Get(competitionDetails.Type.Id);
            return competitionDetails;
        }

        public void UpdatePlayersPosition(int[] competitionIds)
        {
            competitionIds.ForEach(UpdateCompetitionPlayersPosition);
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
