using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

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
            var playersToAdd = new List<AddCompetitionPlayerInfo>();
            for (var i = 0; i < players.Length; i++)
            {
                players[i].Id = playerIds[i];
                var addInfo = new AddCompetitionPlayerInfo();
                addInfo.Player = players[i];
                addInfo.Source = players[i].Source;
                addInfo.CompetitionReferenceId = players[i].CompetitionReferenceId;
                playersToAdd.Add(addInfo);
            }
            return playersToAdd.ToArray();
        }


        public void UpdateCompetitionPlayers(int competitionId, string playersFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(playersFileUrl, "playersFileUrl");
            
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var playersToAdd = GetCompetitionPlayersToAdd(playersFileUrl, competitionsEngine);

            Requires.IntArgumentPositive(competitionId, "competitionId");

            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            if (!competitionsRepository.DoesCompetitionExists(competitionId))
            {
                throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
            }

            competitionsEngine.AddPlayersToCompetition(competitionId, playersToAdd);


        }

        public LoadCompetitionsValidationResult ValidateCompetitionsFile(string competitionsFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionsFileUrl, "competitionsFileUrl");
            var result = LoadCompetitionsValidationResult.Valid;
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var competitions = default(CreateCompetitionInfo[]);
            try
            {
                competitions = competitionsEngine.GetCompetitions(competitionsFileUrl);
            }
            catch (Exception exception)
            {
                result = LoadCompetitionsValidationResult.InvalidFile;
            }
            if (competitions.NotNullOrEmpty())
            {
                var competitionTypes = competitions.Select(c => c.TypeId).Distinct().ToArray();
                if (!competitionsEngine.AreAllValidCompetitionTypes(competitionTypes))
                {
                    result = LoadCompetitionsValidationResult.InvalidCompetitionType;
                }
            }
            return result;
        }

        public void LoadCompetitionsPlayers(string competitionsPlayersFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionsPlayersFileUrl, "competitionsFileUrl");

            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var playersToAdd = GetCompetitionPlayersToAdd(competitionsPlayersFileUrl, competitionsEngine);
            foreach(var playersGroup in playersToAdd.Where(p=>p.CompetitionReferenceId.NotNullOrEmpty()).GroupBy(p=>p.CompetitionReferenceId))
            {
                var competitionId = competitionsRepository.GetCompetitionIdByReferenceId(playersGroup.Key);
                if (competitionId.HasValue)
                {
                    competitionsEngine.AddPlayersToCompetition(competitionId.Value, playersGroup.ToArray());
                }
            }
        }

        public void LoadCompetitions(string competitionsFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionsFileUrl, "competitionsFileUrl");
            var validationResult = ValidateCompetitionsFile(competitionsFileUrl);
            if (validationResult != LoadCompetitionsValidationResult.Valid)
            {
                throw new ArgumentException("Competitions file is invalid, please validate first.");
            }
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

                    if (scoreUpdateInfo.SetScores.NotNullOrEmpty())
                    {
                        competitionsEngine.UpdateMatchScore(scoreUpdateInfo);
                    }
                    if (scoreUpdateInfo.Result.HasValue)
                    {
                        competitionsEngine.UpdateMatchResult(scoreUpdateInfo);
                    }
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

            if ((int)competition.Status > (int)CompetitionStatus.Started)
            {
                throw new ArgumentException("Invalid status transition from '{1}' to Positioned, Competition Id: {0}.".ParseTemplate(id, competition.Status));
            }

            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            competitionsEngine.UpdatePlayersPosition(id, section);

            competitionsEngine.QualifyByeMatches(id, section);

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
        public void RemovePlayer(int competitionId, int playerId, CompetitionPlayerStatus status, string reason)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            competitionsEngine.RemovePlayerFromUnplayedMatches(competitionId, playerId);
            competitionsEngine.RemovePlayerFromCompetition(competitionId, playerId, status, reason);
        }

        public void ReplacePlayer(int competitionId, int replacedPlayerId, int replacingPlayerId, CompetitionPlayerSource source, CompetitionPlayerStatus status, string reason)
        {
            var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();
            var player = competitionRepository.GetCompetitionPlayer(competitionId, replacedPlayerId);
            if (player.IsNull())
            {
                throw new ArgumentException("Cannot replace player, player does not exist.");
            }

            RemovePlayer(competitionId, replacedPlayerId, status, reason);
            AddPlayerToCompetition(competitionId, replacingPlayerId, source, player.Section);
        }

        public void AddPlayerToCompetition(int competitionId, int playerId, CompetitionPlayerSource source, CompetitionSection section)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var player = playersRepository.Get(playerId);
            var competition = competitionsRepository.GetCompetition(competitionId);
            if (competition == null)
            {
                throw new ArgumentException("Invalid competition id");
            }
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

            if ((int)competition.Status >= (int)CompetitionStatus.Positioned)
            {
                competitionsEngine.PositionPlayerInSection(competitionId, playerId, section);
            }

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
