using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using Rhino.Etl.Core.Files;
using Simple.ComponentModel;
using Simple.Data;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;
using Simple.SAMS.Utilities;

namespace Simple.SAMS.Competitions.Services
{
    public class CompetitionEngineService : ICompetitionsEngine
    {

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

        public void AddPlayersToCompetition(int competitionId, Contracts.Players.Player[] players)
        {

            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            var competition = competitionsRepository.GetCompetition(competitionId);
            if (competition.IsNull())
            {
                throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
            }
            var competitionType = competitionTypesRepository.Get(competition.Type.Id);
            var playersToMatch = players.ToArray();
            var playerIds = playersRepository.MatchPlayerByIdNumber(playersToMatch);
            var competitionPlayers = new List<PlayerInCompetition>();
            for (var i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var rank = competitionType.RankPlayer(player);
                var playerInCompetition = new PlayerInCompetition() { PlayerId = playerIds[i], Rank = rank };
                competitionPlayers.Add(playerInCompetition);
            }
            
            competitionsRepository.AddPlayersToCompetition(competitionId, competitionPlayers.ToArray());
        }

        public Player[] GetCompetitionPlayers(string playersFileUrl)
        {
            var fileName = DownloadFile(playersFileUrl);
            var players = LoadPlayersFromFile(fileName);

            return players.Select(p => new Player
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
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            var competitionDetails = competitionsRepository.GetCompetitionDetails(competitionId);
            var competitionType = competitionTypesRepository.Get(competitionDetails.Type.Id);

            var positioningEngineFactory = ServiceProvider.Get<IPositioningEngineFactory>();
            var positioningEngine = positioningEngineFactory.Create(competitionType.Method);
            if (positioningEngine.IsNull())
            {
                throw new ApplicationException("Positioning engine factory '{0}' returned null, instance of {1} is expected.".ParseTemplate(positioningEngineFactory.GetType().FullName, typeof(IPositioningEngine).FullName));
            }

            var positions = positioningEngine.PositionPlayers(competitionDetails, competitionType);

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
