using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Utilities
{
    public class CompetitionDrawGenerator
    {
        private readonly string m_targetPath;

        public CompetitionDrawGenerator(string targetPath)
        {
            m_targetPath = targetPath;
        }

        private void GeneratePdf(string source, string target)
        {
            var pcdd = ConfigurationManager.AppSettings["Generator.Path"];
            if (string.IsNullOrEmpty(pcdd))
            {
                var pi = new ProcessStartInfo(pcdd);
                pi.Arguments = @"""" + source + @""" """ + target + @"""";
                pi.CreateNoWindow = true;
                Process.Start(pi);
                
            }
        }

        public string Generate(CompetitionDetails details, CompetitionSection section)
        {
            var name = string.Format("{0}-{1}-{2}-{3}", details.Name, details.Id, Guid.NewGuid(), section);
            var outputPath = Path.Combine(m_targetPath, name + ".pdf");
            var infoPath = Path.Combine(m_targetPath, name + ".pcdf");
            var playersPath = Path.Combine(m_targetPath, name + ".players.csv");
            var schedulePath = Path.Combine(m_targetPath, name + ".schedule.csv");
            var inverter = new HebrewWordInverter();
            var info = GetGenerateInfo(details, inverter);
            
            var players = details.Players.Where(p => p.Section == section).ToArray();
            var matches = details.Matches.Where(m => m.Section == section).ToArray();
            SavePlayers(players, playersPath, inverter);
            SaveSchedule(matches, players, schedulePath);
            info.DataParticipantsFile = playersPath;
            info.DataScheduleFile = schedulePath;

            info.Write(infoPath);
            GeneratePdf(infoPath, outputPath);
            return outputPath;
        }

        private void SaveSchedule(MatchHeaderInfo[] matches, CompetitionPlayer[] players, string schedulePath)
        {
            var playersMap = new Dictionary<int, int>();
            var index = 1;
            foreach (var player in players)
            {
                playersMap[player.Id] = index++;
            }

            var items = new List<MatchScheduleInfo>();
            index = 1;
            foreach (var match in matches)
            {
                var matchSchedule = new MatchScheduleInfo();
                matchSchedule.Id = "M" + index++;
                if (match.Player1 != null)
                {
                    matchSchedule.Player1 = playersMap[match.Player1.Id];
                }
                else if (match.Player1 == null || match.Player1Code == "BYE")
                {
                    matchSchedule.Player1 = players.Length+1;
                }
                if (match.Player2 != null)
                {
                    matchSchedule.Player2 = playersMap[match.Player2.Id];
                }
                else if (match.Player2 == null || match.Player2Code == "BYE")
                {
                    matchSchedule.Player2 = players.Length+1;
                }

                if (match.Winner != MatchWinner.None)
                {
                    matchSchedule.Winner = (int) match.Winner;
                }

                matchSchedule.Player1Points = 0;
                matchSchedule.Player2Points = 0;
                matchSchedule.SetScores = match.SetScores;
                //matchSchedule.SetScores = new[]
                //                              {
                //                                  new SetScore()
                //                                      {
                //                                          Player1Points = 7,
                //                                          Player2Points = 5,
                //                                      }, 
                //                                  new SetScore()
                //                                      {
                //                                          Player1Points = 2,
                //                                          Player2Points = 6,
                //                                      }, 
                //                                  new SetScore()
                //                                      {
                //                                          Player1Points = 0,
                //                                          Player2Points = 6,
                //                                      }, 
                //                              };
                items.Add(matchSchedule);
            }

            MatchScheduleInfo.Write(items.ToArray(), schedulePath);
        }

        private void SavePlayers(CompetitionPlayer[] players, string playersPath, HebrewWordInverter inverter)
        {
            File.WriteAllLines(playersPath, players.Select(p=>string.Format("{0};;;", inverter.Invert(p.FullName))));
        }

        

        private DrawGeneratorInfo GetDefaultGenerateInfo()
        {
            var path = Path.Combine(Path.GetTempPath(), "Simple.SAMS.Defaults.pcdf");
            using (var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Defaults.pcdf"))
            using(var fileStream = File.OpenWrite(path))
            {
                stream.CopyTo(fileStream);
            }

            var result = new DrawGeneratorInfo();
            result.Read(path);
            return result;
        }

        private DrawGeneratorInfo GetGenerateInfo(CompetitionDetails details, HebrewWordInverter inverter)
        {
            var result = GetDefaultGenerateInfo();
            
            result.DrawTitle = inverter.Invert(details.Name);
            result.DrawType = inverter.Invert(details.SitePhone) + ", " + inverter.Invert(details.Site);
            result.DrawOtherInfo = inverter.Invert(details.MainRefereePhone) + ", " + inverter.Invert(details.MainRefereeName);
            result.DocumentAuthor = "Simple. SAMS - ITA " + DateTime.Now.Year.ToString();
            
            return result;
        }
    }
}
