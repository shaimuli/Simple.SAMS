using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Competitions.Services
{
    public class CompetitionsManagerService : ICompetitionsManager
    {
        public void Create(CreateCompetitionInfo competitionCreateInfo)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
                var competitionId = competitionsEngine.CreateCompetition(competitionCreateInfo);

                var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
                var competitionHeaderInfo = competitionsRepository.GetCompetition(competitionId);
                competitionsEngine.CreateCompetitionsMatches(new[] { competitionHeaderInfo });

                if (competitionCreateInfo.PlayersFileUrl.NotNullOrEmpty())
                {
                    UpdateCompetitionPlayers(competitionId, competitionCreateInfo.PlayersFileUrl);
                }

                scope.Complete();
            }

        }

        private AddCompetitionPlayerInfo[] GetCompetitionPlayersToAdd(string playersFile, ICompetitionsEngine competitionsEngine)
        {
            var players = competitionsEngine.GetCompetitionPlayers(playersFile);
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var playersToMatch = players.ToArray();
            var playerIds = playersRepository.MatchPlayerByIdNumber(playersToMatch);
            var playersToAdd = new Boo.Lang.List<AddCompetitionPlayerInfo>();
            for (var i = 0; i < players.Length; i++)
            {
                players[i].Id = playerIds[i];
                var addInfo = new AddCompetitionPlayerInfo();
                addInfo.Player = players[i];
                addInfo.Source = players[i].Source;
                playersToAdd.Add(addInfo);
            }
            return playersToAdd.ToArray();
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
            var playersToAdd = GetCompetitionPlayersToAdd(playersFileUrl, competitionsEngine);
            competitionsEngine.AddPlayersToCompetition(competitionId, playersToAdd);


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
            var updatedCompetitionIds = new HashSet<int>();

            scoreUpdateInfoItems.ForEach(
                scoreUpdateInfo =>
                {
                    if (scoreUpdateInfo.SetScores.IsNullOrEmpty())
                    {
                        throw new ArgumentException("You must specify set scores.");
                    }

                    competitionsEngine.UpdateMatchScore(scoreUpdateInfo);

                    var competitionMatchesRepository = ServiceProvider.Get<ICompetitionMatchesRepository>();
                    var competitionId = competitionMatchesRepository.GetMatchCompetitionId(scoreUpdateInfo.MatchId);
                    updatedCompetitionIds.Add(competitionId);
                    if (scoreUpdateInfo.Winner.HasValue)
                    {
                        competitionsEngine.QualifyMatchWinner(scoreUpdateInfo.MatchId);
                    }
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

            if (competition.Status != CompetitionStatus.Finished)
            {
                if (competition.Status != CompetitionStatus.Started)
                {
                    throw new ArgumentException(
                        "Competition '{0}' must be in status '{1}' in order to be started.".ParseTemplate(id,
                                                                                                          CompetitionStatus
                                                                                                              .Started));
                }

                repository.UpdateCompetitionStatus(id, CompetitionStatus.Finished);
            }
        }

        public void PositionCompetitionPlayers(int id, CompetitionSection section)
        {
            var repository = ServiceProvider.Get<ICompetitionRepository>();
            var competition = repository.GetCompetition(id);
            if (competition.IsNull())
            {
                throw new ArgumentException("Competition '{0}' does not exist.".ParseTemplate(id));
            }

            if ((int)competition.Status != (int)CompetitionStatus.MatchesCreated)
            {
                throw new ArgumentException("Invalid status transition from '{1}' to Positioned, Competition Id: {0}.".ParseTemplate(id, competition.Status));
            }

            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            competitionsEngine.UpdatePlayersPosition(id, section);

        }

        public void StartCompetition(int id)
        {
            var repository = ServiceProvider.Get<ICompetitionRepository>();
            var competition = repository.GetCompetition(id);
            if (competition.IsNull())
            {
                throw new ArgumentException("Competition '{0}' does not exist.".ParseTemplate(id));
            }
            if ((int)competition.Status != (int)CompetitionStatus.Positioned)
            {
                throw new ArgumentException("Invalid status transition from '{1}' to Started, Competition Id: {0}.".ParseTemplate(id, competition.Status));
            }

            repository.UpdateCompetitionStatus(id, CompetitionStatus.Started);
        }
        public void RemovePlayer(int competitionId, int playerId)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            competitionsEngine.RemovePlayerFromUnplayedMatches(competitionId, playerId);
            competitionsEngine.RemovePlayerFromCompetition(competitionId, playerId);
        }

        public void ReplacePlayer(int competitionId, int replacedPlayerId, int replacingPlayerId)
        {
            var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();
            var player = competitionRepository.GetCompetitionPlayer(competitionId, replacedPlayerId);
            if (player.IsNull())
            {
                throw new ArgumentException("Cannot replace player, player does not exist.");
            }

            RemovePlayer(competitionId, replacedPlayerId);
            AddPlayerToCompetition(competitionId, replacingPlayerId, player.Source, player.Section);
        }

        public void AddPlayerToCompetition(int competitionId, int playerId, CompetitionPlayerSource source, CompetitionSection section)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var player = playersRepository.Get(playerId);

            competitionsEngine.AddPlayersToCompetition(competitionId,
                new[]
                    {
                        new AddCompetitionPlayerInfo()
                            {
                                Player = player, 
                                Source = source,
                                Section = section
                            }
                    });
            competitionsEngine.PositionPlayerInSection(competitionId, playerId, section);
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
