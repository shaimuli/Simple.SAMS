using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;

namespace Simple.SAMS.Competitions.Services
{
    public class KnockoutPositioningEngine : IPositioningEngine
    {
        public Contracts.Competitions.UpdatePlayerPositionInfo[] PositionPlayers(Contracts.Competitions.CompetitionDetails details, Contracts.Competitions.CompetitionType competitionType)
        {
            var players = details.Players.OrderBy(p=>p.CompetitionRank).Take(competitionType.PlayersCount).ToList();
            var orderedMatches = details.Matches.OrderBy(m => m.Position).ToList();
            var positions = new List<UpdatePlayerPositionInfo>();
            var matchIndex = 0;
            while (players.Count>0)
            {
                var player = players[0];
                if (matchIndex>orderedMatches.Count)
                {
                    throw new ApplicationException("Matches count is insufficient.");
                }
                var match = orderedMatches[matchIndex];
                var position = 0;
                
                if (match.Player1 == null)
                {
                    match.Player1 = new MatchPlayer() {Id = player.Id};
                }
                else if (match.Player2 == null)
                {
                    match.Player2 = new MatchPlayer() { Id = player.Id };
                    position = 1;
                    matchIndex++;
                }

                var updateInfo = new UpdatePlayerPositionInfo() { MatchId = match.Id, PlayerId = player.Id, Position = position };
                positions.Add(updateInfo);
                players.RemoveAt(0);
            }

            //var finalMatches = orderedMatches.Where(m => m.Section == CompetitionSection.Final);
            //var qualifyingMatches = orderedMatches.Where(m => m.Section == CompetitionSection.Qualifying);

            
            //if (competitionType.WildcardPlayersCount > 0)
            //{
            //    var topPlayers =
            //        players.OrderBy(p => p.CompetitionRank).Take(competitionType.WildcardPlayersCount);

            //    //positions.Add(new UpdatePlayerPositionInfo(){ MatchId = finalMatches.First().Id, PlayerId = topPlayers.First() })
                

            //    // TBD

            //    //var firstMatch = finalMatches.FirstOrDefault();
            //    //if (firstMatch.IsNull())
            //    //{
            //    //    throw new ApplicationException("At least one match is required in final section when wildcard players count larger than 0.");
            //    //}
            //    //var firstPlayer = topPlayers.FirstOrDefault();
            //    //if (firstPlayer.IsNull())
            //    //{
            //    //    throw new ApplicationException("At least one player is required in final section when wildcard players count larger than 0.");
            //    //}

            //    //positions.Add(new UpdatePlayerPositionInfo(){ MatchId = firstMatch.Id, PlayerId = firstPlayer.Id });


            //}

            return positions.ToArray();
        }
    }
}
