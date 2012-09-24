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
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Competitions.Services
{
    public class CompetitionEngineService : ICompetitionsEngine
    {

        public void AddPlayersToCompetition(int competitionId, Contracts.Players.Player[] players)
        {
            
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var playersToMatch = players.Select(p => new Player {IdNumber = p.IdNumber, Name = p.Name}).ToArray();
            var playerIds = playersRepository.MatchPlayerByIdNumber(playersToMatch);
            var competitionPlayers = new List<PlayerInCompetition>();
            for (var i = 0; i < players.Length; i++)
            {
                var playerInCompetition = new PlayerInCompetition() {PlayerId = playerIds[i], Rank = players[i].Rank};
                competitionPlayers.Add(playerInCompetition);
            }
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            competitionsRepository.AddPlayersToCompetition(competitionId, competitionPlayers.ToArray());
        }

        public Player[] GetCompetitionPlayers(string playersFileUrl)
        {
            var fileName = DownloadFile(playersFileUrl);
            var players = LoadPlayersFromFile(fileName);

            return players.Select(p => new Player { IdNumber = p.IdNumber, Name = p.Name, Rank = p.Rank }).ToArray();
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
            var competitionHeaderInfo = new CompetitionHeaderInfo();
            competitionHeaderInfo.ReferenceId = Guid.NewGuid().ToString();
            competitionHeaderInfo.Name = competitionCreateInfo.Name;
            competitionHeaderInfo.Type = new EntityReference() { Id = competitionCreateInfo.TypeId };
            competitionHeaderInfo.StartTime = competitionCreateInfo.StartTime;

            var result = repository.Create(competitionHeaderInfo);

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

            [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")] public DateTime StartTime;
        }

        [DelimitedRecord(","), IgnoreFirst]
        private class PlayerRecord
        {
            public string IdNumber { get; set; }
            public string Name { get; set; }
            public int Rank { get; set; }
        }



        public CompetitionHeaderInfo[] GetCompetitions(string competitionsFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionsFileUrl, "competitionsFileUrl");
            var fileName = DownloadFile(competitionsFileUrl);
            var competitionRecords = LoadCompetitionsFromFile(fileName);
            return competitionRecords.Select(cr=> new CompetitionHeaderInfo(){Name = cr.Name, StartTime = cr.StartTime, Type = new EntityReference(){ Id= MapCompetitionTypeId(cr.TypeId) }, ReferenceId = cr.ReferenceId}).ToArray();
        }

        private int MapCompetitionTypeId(int competitionRecordTypeId)
        {
            return competitionRecordTypeId;
        }


        public void ImportCompetitions(CompetitionHeaderInfo[] competitions)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            competitionsRepository.UpdateCompetitionsByReferenceId(competitions);
        }
    }
}
