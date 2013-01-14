using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Competitions.Data
{
    public static class MatchHeaderMappingExtensions
    {
        public static MatchHeaderInfo MapFromData(this Match match)
        {
            var result = new MatchHeaderInfo()
            {
                Id = match.Id,
                StartTime = match.StartTime.HasValue ? DateTime.SpecifyKind(match.StartTime.Value, DateTimeKind.Utc).ToLocalTime() : default(DateTime?),
                Status = (MatchStatus)match.Status,
                Section = (CompetitionSection)match.SectionId,
                CompetitionId = match.CompetitionId,
                StartTimeType = (StartTimeType)match.StartTimeType,
                Round = match.Round,
                RoundRelativePosition = match.RoundRelativePosition,
                Position = match.Position,
                IsFinal = match.IsFinal,
                IsSemiFinal = match.IsSemiFinal
            };
            if (match.Winner.HasValue)
            {
                result.Winner = (MatchWinner)match.Winner.Value;
            }
            if (match.Result.HasValue)
            {
                result.Result = (MatchResult)match.Result.Value;
            }

            result.Player1 = CreateMatchPlayerFromData(match.Player);
            result.Player2 = CreateMatchPlayerFromData(match.Player5);
            result.Player3 = CreateMatchPlayerFromData(match.Player6);
            result.Player4 = CreateMatchPlayerFromData(match.Player7);
            result.SetScores =
                match.MatchScores.Select(ms => new SetScore()
                {
                    Number = ms.SetNumber,
                    Player1Points = ms.Player1Points,
                    Player2Points = ms.Player2Points,
                    BreakPoints = ms.BreakPoints
                }).ToArray();

            return result;        
        }

        private static Contracts.Competitions.MatchPlayer CreateMatchPlayerFromData(Player player)
        {
            var result = default(Contracts.Competitions.MatchPlayer);
            if (player.IsNotNull())
            {
                result = new Contracts.Competitions.MatchPlayer();

                result.Id = player.Id;
                result.IdNumber = player.IdNumber;

                result.LocalFirstName = player.LocalFirstName;
                result.LocalLastName = player.LocalLastName;
                result.EnglishFirstName = player.EnglishFirstName;
                result.EnglishLastName = player.EnglishLastName;
            }
            return result;
        }
    }
}
