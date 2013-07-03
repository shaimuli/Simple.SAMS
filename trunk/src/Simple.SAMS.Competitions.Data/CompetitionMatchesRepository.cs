using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.ComponentModel;
using Simple.Data;
using Simple.SAMS.Contracts.Competitions;
using Simple.SimplyLog.Data;
using Simple.Utilities;

namespace Simple.SAMS.Competitions.Data
{
    public class CompetitionMatchesRepository : DataRepositoryBase<CompetitionsDataContext>, ICompetitionMatchesRepository
    {
        public int GetRoundMatchesCount(int competitionId, CompetitionSection section, int round)
        {
            var result = 0;
            UseDataContext(
                dataContext =>
                    {
                        result =
                            dataContext.Matches.Count(
                                m =>
                                m.CompetitionId == competitionId && m.SectionId == (int) section && m.Round == round);
                    });
            return result;
        }

        public int GetCompetitionSectionRounds(int competitionId, CompetitionSection section)
        {
            var rounds = 6;
            UseDataContext(
                dataContext =>
                    {
                        var minRound = dataContext.Matches.Where(m => m.CompetitionId == competitionId && m.SectionId == (int)section).Min(m => m.Round);
                        rounds = 6 - minRound;
                    });

            return rounds;
        }

        public int GetMatchCompetitionId(int matchId)
        {
            var competitionId = default(int);

            UseDataContext(
                dataContext =>
                    {

                        var match = dataContext.Matches.SingleOrDefault(m => m.Id == matchId);
                        if (match == null)
                        {
                             throw new ArgumentException("Invalid match id");
                        }

                        competitionId = match.CompetitionId;
                    });

            return competitionId;
        }
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
            dataMatch.SlotPosition = match.SlotPosition;
            if (match.SlotType.HasValue)
            {
                dataMatch.SlotType = (int)match.SlotType.Value;
            }
            else
            {
                dataMatch.SlotType = default(int?);
            }
            dataMatch.SectionId = (int)match.Section;
            dataMatch.Position = match.Position;
            dataMatch.Round = match.Round;
            dataMatch.RoundRelativePosition = match.RoundRelativePosition;
            dataMatch.Status = (int)match.Status;
            dataMatch.StartTimeType = (int)match.StartTimeType;
            dataMatch.IsFinal = match.IsFinal;
            dataMatch.IsSemiFinal = match.IsSemiFinal;
            dataMatch.Player1Code = match.Player1Code;
            dataMatch.Player2Code = match.Player2Code;
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
                        if (match.IsNotNull())
                        {
                            // !!!!
                            if (match.Status < (int) MatchStatus.Planned)
                            {
                                match.Status = (int) MatchStatus.Planned;
                            }
                            match.StartTime = startTimeUpdateInfo.StartTime.ToUniversalTime();
                            match.StartTimeType = (int) startTimeUpdateInfo.StartTimeType;

                            dataContext.SubmitChanges();
                        }
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
                                dataSetScore.Match = dataContext.Matches.First(m => m.Id == matchId);
                                dataContext.MatchScores.InsertOnSubmit(dataSetScore);
                                
                                dataSetScores[setScore.Number] = dataSetScore;
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
                            if (
                                dataSetScore.Match.Status < (int) MatchStatus.Playing)
                            {
                                dataSetScore.Match.Status = (int) MatchStatus.Playing;
                            }
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
 
                dataContext.SubmitChanges();
            });
                
            }
        }

        public void UpdateMatchResult(MatchScoreUpdateInfo scoreUpdateInfo)
        {
            UseDataContext(
                dataContext =>
                    {

                        var match = dataContext.Matches.FirstOrDefault(m => m.Id == scoreUpdateInfo.MatchId);
                        if (match == null)
                        {
                            throw new ArgumentException(
                                "Match '{0}' could not be found, could not update score.".ParseTemplate(
                                    scoreUpdateInfo.MatchId));
                        }

                        if (scoreUpdateInfo.Winner.HasValue)
                        {
                            match.Winner = scoreUpdateInfo.Winner.Value == MatchWinner.None
                                               ? default(int?)
                                               : (int) scoreUpdateInfo.Winner.Value;

                            if (match.Winner.HasValue)
                            {
                                match.Status = (int) MatchStatus.Completed;
                            }
                        }
                        if (scoreUpdateInfo.Result.HasValue)
                        {
                            match.Result = (int) scoreUpdateInfo.Result.Value;
                        }
                        
                        dataContext.SubmitChanges();
                    });
        }

        public void RemovePlayerFromUnplayedMatches(int competitionId, int playerId)
        {
            UseDataContext(
                dataContext =>
                    {
                        var relevantMatches = dataContext.Matches.Where(m =>
                                                                        m.CompetitionId == competitionId &&
                                                                        m.Status <= (int) MatchStatus.Planned);

                        var matchesWithPlayer1 = relevantMatches.Where(m => m.Player1 == playerId);
                        var matchesWithPlayer2 = relevantMatches.Where(m => m.Player2 == playerId);
                        var matchesWithPlayer3 = relevantMatches.Where(m => m.Player3 == playerId);
                        var matchesWithPlayer4 = relevantMatches.Where(m => m.Player4 == playerId);

                        matchesWithPlayer1.ForEach(
                            match =>
                                {
                                    match.Player1 = default(int?);
                                });

                        matchesWithPlayer2.ForEach(
                            match =>
                                {
                                    match.Player2 = default(int?);
                                });

                        matchesWithPlayer3.ForEach(
                            match =>
                                {
                                    match.Player3 = default(int?);
                                });

                        matchesWithPlayer4.ForEach(
                            match =>
                                {
                                    match.Player4 = default(int?);
                                });

                        dataContext.SubmitChanges();
                    });
        }


        public MatchHeaderInfo GetMatch(int matchId)
        {
            var result = default(MatchHeaderInfo);

            UseDataContext(
                dataContext =>
                    {
                        var match = dataContext.Matches.FirstOrDefault(m => m.Id == matchId);
                        if (match.IsNull())
                        {
                            throw new ArgumentException("Match '{0}' could not be found.".ParseTemplate(matchId));
                        }

                        result = match.MapFromData();
                    });

            return result;
        }

        public MatchHeaderInfo GetMatchByRelativePosition(int competitionId, CompetitionSection section, int round, int relativePosition)
        {
            var result = default(MatchHeaderInfo);
            UseDataContext(
                dataContext =>
                {
                    var match = dataContext.Matches.FirstOrDefault(m => m.CompetitionId == competitionId && m.SectionId == (int)section && m.Round == round && m.RoundRelativePosition == relativePosition);
                    
                    result = match.MapFromData();
                });
            return result;
        }
        public MatchHeaderInfo GetMatchByPosition(int competitionId, CompetitionSection section, int position)
        {
            var result = default(MatchHeaderInfo);
            UseDataContext(
                dataContext =>
                {
                    var match = dataContext.Matches.FirstOrDefault(m => m.CompetitionId == competitionId && m.SectionId == (int)section && m.Position == position);
                    
                    result = match.MapFromData();
                });
            return result;
        }
    }
}
