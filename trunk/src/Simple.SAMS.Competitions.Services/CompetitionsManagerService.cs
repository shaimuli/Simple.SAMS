using System;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

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

        public void UpdateMatchScore(MatchScoreUpdateInfo[] scoreUpdateInfoItems)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();

            scoreUpdateInfoItems.ForEach(
                scoreUpdateInfo =>
                    {
                        if (scoreUpdateInfo.SetScores.IsNullOrEmpty())
                        {
                            throw new ArgumentException("You must specify set scores.");
                        }

                        competitionsEngine.UpdateMatchScore(scoreUpdateInfo);
                    });
            
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

        public void UpdateMatchStartTime(MatchStartTimeUpdateInfo[] updates)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();            
            updates.ForEach(
                startTimeUpdateInfo =>
                    {
                        Requires.IntArgumentPositive(startTimeUpdateInfo.MatchId, "startTimeUpdateInfo.MatchId");
                        
                        competitionsEngine.UpdateMatchStartTime(startTimeUpdateInfo);
                    });
     
        }
    }
}
