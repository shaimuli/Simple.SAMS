using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Data;
using Simple.SAMS.Contracts.Competitions;
using Simple.SimplyLog.Data;
using Simple.Utilities;

namespace Simple.SAMS.Competitions.Data
{
    public class CompetitionMatchesRepository : DataRepositoryBase<CompetitionsDataContext>, ICompetitionMatchesRepository
    {
        public void AddCompetitionMatches(int competitionId, MatchHeaderInfo[] matches)
        {
            UseDataContext(
                dataContext =>
                {
                    var competition = dataContext.Competitions.FirstOrDefault(c => c.Id == competitionId);
                    if (competition.IsNull())
                    {
                        throw new ArgumentException(
                            "Competition '{0}' could not be found.".ParseTemplate(competitionId));
                    }

                    competition.Status = (int)CompetitionStatus.MatchesCreated;

                    matches.ForEach(
                        match =>
                        {
                            var dataMatch = new Match();
                            dataMatch.CompetitionId = competitionId;
                            dataMatch.Created = dataMatch.Updated = DateTime.UtcNow;
                            dataMatch.RowStatus = 0;

                            MapMatchToDataMatch(match, dataMatch);

                            dataContext.Matches.InsertOnSubmit(dataMatch);
                        });

                    dataContext.SubmitChanges();
                });
        }

        private void MapMatchToDataMatch(MatchHeaderInfo match, Match dataMatch)
        {
            dataMatch.SectionId = (int)match.Section;
            dataMatch.Position = match.Position;
            dataMatch.Round = match.Round;
            dataMatch.Status = (int)match.Status;
            dataMatch.StartTimeType = (int)match.StartTimeType;
        }


        public void UpdatePlayersPosition(int competitionId, UpdatePlayerPositionInfo[] positions)
        {
            UseDataContext(
                dataContext =>
                {
                    var competition = dataContext.Competitions.FirstOrDefault(c => c.Id == competitionId);
                    if (competition == null)
                    {
                        throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
                    }

                    competition.Status = (int)CompetitionStatus.Positioned;
                    var matches = dataContext.Matches.Where(m => m.CompetitionId == competitionId).ToArray();
                    var matchesMap = new Dictionary<int, Match>();
                    matches.ForEach(match =>
                                        {
                                            matchesMap[match.Id] = match;
                                        });
                    positions.ForEach(position =>
                                          {
                                              var match = matchesMap[position.MatchId];
                                              if (position.Position == 0)
                                              {
                                                  match.Player1 = position.PlayerId;
                                              }
                                              if (position.Position == 1)
                                              {
                                                  match.Player2 = position.PlayerId;
                                              }
                                              if (position.Position == 2)
                                              {
                                                  match.Player3 = position.PlayerId;
                                              }
                                              if (position.Position == 3)
                                              {
                                                  match.Player4 = position.PlayerId;
                                              }
                                              match.Status = (int)MatchStatus.PlayersAssigned;
                                          });

                    dataContext.SubmitChanges();
                });
        }

        private readonly object m_lock = new object();
        public void UpdateMatchStartTime(MatchStartTimeUpdateInfo startTimeUpdateInfo)
        {
            UseDataContext(
                dataContext =>
                    {
                        var match = dataContext.Matches.FirstOrDefault(m=>m.Id == startTimeUpdateInfo.MatchId);
                        if (match.Status < (int)MatchStatus.Planned)
                        {
                            match.Status = (int) MatchStatus.Planned;
                        }
                        match.StartTime = startTimeUpdateInfo.StartTime.ToUniversalTime();
                        match.StartTimeType = (int) startTimeUpdateInfo.StartTimeType;

                        dataContext.SubmitChanges();
                    });
        }

        public void UpdateMatchScore(MatchScoreUpdateInfo scoreUpdateInfo)
        {
            if (scoreUpdateInfo.SetScores.IsNullOrEmpty())
            {
                throw new ArgumentException("You must specify set scores.");
            }
            lock (m_lock)
            {
                UseDataContext(
            dataContext =>
            {
                var matchId = scoreUpdateInfo.MatchId;
                var dataSetScores = new Dictionary<int, MatchScore>();
                scoreUpdateInfo.SetScores.ForEach(
                    setScore =>
                    {
                        var dataSetScore =
                            dataContext.MatchScores.FirstOrDefault(ms => ms.MatchId == matchId && ms.SetNumber == setScore.Number);
                        if (dataSetScore == null)
                        {
                            if (!dataSetScores.TryGetValue(setScore.Number, out dataSetScore))
                            {
                                dataSetScore = new MatchScore();
                                dataSetScore.SetNumber = setScore.Number;
                                dataSetScore.MatchId = matchId;
                                dataContext.MatchScores.InsertOnSubmit(dataSetScore);
                                dataSetScores[setScore.Number] = dataSetScore;
                            }
                        }
                        dataSetScore.SetNumber = setScore.Number;
                        if (setScore.Player1Points.HasValue)
                        {
                            dataSetScore.Player1Points = setScore.Player1Points.Value;
                        }
                        if (setScore.Player2Points.HasValue)
                        {
                            dataSetScore.Player2Points = setScore.Player2Points.Value;
                        }
                        if (setScore.BreakPoints.HasValue)
                        {
                            dataSetScore.BreakPoints = setScore.BreakPoints.Value;
                        }

                    });

                dataContext.SubmitChanges();

                var match = dataContext.Matches.FirstOrDefault(m => m.Id == matchId);
                if (match == null)
                {
                    throw new ArgumentException("Match '{0}' could not be found, could not update score.".ParseTemplate(scoreUpdateInfo.MatchId));
                }

                var latest = scoreUpdateInfo.SetScores.OrderByDescending(s => s.Number).First();
                match.Id = matchId;
                if (latest.Player1Points.HasValue)
                {
                    match.Player1Points = latest.Player1Points.Value;
                }
                if (latest.Player2Points.HasValue)
                {
                    match.Player2Points = latest.Player2Points.Value;
                }
                if (latest.BreakPoints.HasValue)
                {
                    match.BreakPoints = latest.BreakPoints.Value;
                }
                if (scoreUpdateInfo.Winner.HasValue)
                {
                    match.Winner = scoreUpdateInfo.Winner.Value == MatchWinner.None
                                       ? default(int?)
                                       : (int) scoreUpdateInfo.Winner.Value;
                }
                if (scoreUpdateInfo.Result.HasValue)
                {
                    match.Result = (int)scoreUpdateInfo.Result.Value;
                }
                dataContext.SubmitChanges();
            });
                
            }
        }
    }
}
