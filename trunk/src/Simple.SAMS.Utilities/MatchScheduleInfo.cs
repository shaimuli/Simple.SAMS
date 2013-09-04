using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Utilities
{
    public class MatchScheduleInfo
    {
        public string Id { get; set; }
        public int Player1 { get; set; }
        public int Player2 { get; set; }
        public int? Winner { get; set; }
        public int Player1Points { get; set; }
        public int Player2Points { get; set; }
        public SetScore[] SetScores { get; set; }

        public static void Write(MatchScheduleInfo[] items, string targetFileName)
        {
            var lines = new List<string>();
            foreach (var matchScheduleInfo in items)
            {
                var winner = matchScheduleInfo.Winner.HasValue
                                 ? (matchScheduleInfo.Winner == (int)MatchWinner.Player1
                                        ? matchScheduleInfo.Player1
                                        : matchScheduleInfo.Player2).ToString()
                                 : string.Empty;
                
                var detailedResult = matchScheduleInfo.SetScores == null ? string.Empty: string.Join(" ", matchScheduleInfo.SetScores.Select(s=> s.Player1Points + ":" + s.Player2Points));
                var data = new[]
                               {
                                   matchScheduleInfo.Id.ToString(), 
                                   matchScheduleInfo.Player1.ToString(),
                                   matchScheduleInfo.Player2.ToString(), 
                                   winner,
                                   matchScheduleInfo.Player1Points + ":" + matchScheduleInfo.Player2Points, detailedResult,
                                   string.Empty, string.Empty
                               };
                var line = string.Join(";", data);
                
                lines.Add(line);

            }
            File.WriteAllLines(targetFileName, lines);
        }

    }
}
