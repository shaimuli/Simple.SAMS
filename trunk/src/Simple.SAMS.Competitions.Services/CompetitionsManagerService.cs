using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Files;
using Simple.Common.Storage;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;

namespace Simple.SAMS.Competitions.Services
{
    public class CompetitionsManagerService : ICompetitionsManager
    {
        public void Create(CreateCompetitionInfo competitionCreateInfo, string playersFile)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var competitionId = competitionsEngine.CreateCompetition(competitionCreateInfo);

            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var competitionHeaderInfo = competitionsRepository.GetCompetition(competitionId);
            competitionsEngine.CreateCompetitionsMatches(new[] { competitionHeaderInfo });

            if (playersFile.NotNullOrEmpty())
            {
                var players = competitionsEngine.GetCompetitionPlayers(playersFile);
                competitionsEngine.AddPlayersToCompetition(competitionId, players);

                
                competitionsEngine.UpdatePlayersPosition(new[] { competitionId });
            }
        }


        public void UpdateCompetitionPlayers(int competitionId, string playersFileUrl)
        {
            Requires.IntArgumentPositive(competitionId, "competitionId");
            Requires.ArgumentNotNullOrEmptyString(playersFileUrl, "playersFileUrl");
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            if (!competitionsRepository.DoesCompetitionExists(competitionId))
            {
                throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
            }

            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var players = competitionsEngine.GetCompetitionPlayers(playersFileUrl);

            competitionsEngine.AddPlayersToCompetition(competitionId, players);

            competitionsEngine.UpdatePlayersPosition(new[] { competitionId });
        }


        public void LoadCompetitions(string competitionsFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionsFileUrl, "competitionsFileUrl");
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var competitions = competitionsEngine.GetCompetitions(competitionsFileUrl);
            competitionsEngine.ImportCompetitions(competitions);

            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var unprovisionedCompetitions = competitionsRepository.GetCompetitionsByStatus(CompetitionStatus.Created);
            competitionsEngine.CreateCompetitionsMatches(unprovisionedCompetitions);
        }


        public MatchHeaderInfo[] GetCompetitionMatches(CompetitionMatchesQuery matchesQuery)
        {
            throw new NotImplementedException();
        }

        public void UpdateMatchScore(MatchScoreUpdateInfo scoreUpdateInfo)
        {
            if (scoreUpdateInfo.SetScores.IsNullOrEmpty())
            {
                throw new ArgumentException("You must specify set scores.");
            }
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            //var match
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            competitionsEngine.UpdateMatchScore(scoreUpdateInfo);
        }
        public void FinishCompetition(int id)
        {
            var repository = ServiceProvider.Get<ICompetitionRepository>();
            var competition = repository.GetCompetition(id);
            if (competition.IsNull())
            {
                throw new ArgumentException("Competition '{0}' does not exist.".ParseTemplate(id));
            }
            if (competition.Status != CompetitionStatus.Started)
            {
                throw new ArgumentException("Competition '{0}' must be in status '{1}' in order to be started.".ParseTemplate(id, CompetitionStatus.Started));
            }

            repository.UpdateCompetitionStatus(id, CompetitionStatus.Finished);
        }

        public void StartCompetition(int id)
        {
            var repository = ServiceProvider.Get<ICompetitionRepository>();
            var competition = repository.GetCompetition(id);
            if (competition.IsNull())
            {
                throw new ArgumentException("Competition '{0}' does not exist.".ParseTemplate(id));
            }
            if (competition.Status != CompetitionStatus.Positioned)
            {
                throw new ArgumentException("Competition '{0}' must be in status '{1}' in order to be started.".ParseTemplate(id, CompetitionStatus.Positioned));
            }

            repository.UpdateCompetitionStatus(id, CompetitionStatus.Started);
        }
    }
}
