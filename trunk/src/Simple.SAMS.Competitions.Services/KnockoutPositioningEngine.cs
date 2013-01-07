using System.Collections.Generic;
using System.Linq;
using Simple;
using Simple.SAMS.CompetitionEngine;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;

public class KnockoutPositioningEngine : IPositioningEngine
{

    // Methods
    private UpdatePlayerPositionInfo[] GetFinalPlayersPositions(CompetitionDetails details)
    {
        IEnumerable<UpdatePlayerPositionInfo> items = Enumerable.Empty<UpdatePlayerPositionInfo>();
        MatchHeaderInfo[] orderedMatches = (from m in details.Matches
            where m.Section == CompetitionSection.Qualifying
            orderby m.Position
            select m).Take<MatchHeaderInfo>((details.Type.QualifyingPlayersCount / 2)).ToArray<MatchHeaderInfo>();
        CompetitionPlayer[] finalPlayers = (from p in details.Players
            where p.Section == CompetitionSection.Final
            select p).ToArray<CompetitionPlayer>();
        if (finalPlayers.Length > 0)
        {
            items = GetUpdatePlayerPositionInfos(PositionQualifyingPlayers(details, finalPlayers), orderedMatches);
        }
        return items.ToArray<UpdatePlayerPositionInfo>();
    }

    private UpdatePlayerPositionInfo[] GetQualifyingPlayersPositions(CompetitionDetails details)
    {
        IEnumerable<UpdatePlayerPositionInfo> items = Enumerable.Empty<UpdatePlayerPositionInfo>();
        MatchHeaderInfo[] orderedMatches = (from m in details.Matches
            where m.Section == CompetitionSection.Qualifying
            orderby m.Position
            select m).Take<MatchHeaderInfo>((details.Type.QualifyingPlayersCount / 2)).ToArray<MatchHeaderInfo>();
        CompetitionPlayer[] qualifyingPlayers = (from p in details.Players
            where p.Section == CompetitionSection.Qualifying
            select p).ToArray<CompetitionPlayer>();
        if (qualifyingPlayers.Length > 0)
        {
            items = GetUpdatePlayerPositionInfos(PositionQualifyingPlayers(details, qualifyingPlayers), orderedMatches);
        }
        return items.ToArray<UpdatePlayerPositionInfo>();
    }
    private static IEnumerable<UpdatePlayerPositionInfo> GetUpdatePlayerPositionInfos(CompetitionPosition[] qualifyingPlayersPositions, MatchHeaderInfo[] orderedMatches)
    {
        int matchIndex = 0;
        int positionIndex = 0;
        return qualifyingPlayersPositions.Select<CompetitionPosition, UpdatePlayerPositionInfo>(delegate (CompetitionPosition position) {
            UpdatePlayerPositionInfo result = null;
            if (position.PlayerId.HasValue)
            {
                result = new UpdatePlayerPositionInfo {
                    PlayerId = position.PlayerId.Value,
                    MatchId = orderedMatches[matchIndex].Id,
                    Position = positionIndex % 2
                };
            }
            positionIndex++;
            if ((positionIndex % 2) == 0)
            {
                matchIndex++;
            }
            return result;
        });
    }

    private CompetitionPosition[] PositionFinalPlayersInCompetition(CompetitionDetails details)
    {
        var positioningEngine = new FinalPositioningEngine();
        var positioningParameters = new FinalPositioningParameters
        {
            PlayersCount = details.Type.PlayersCount,
            RankedPlayersCount = 2,
            Players = (from p in details.Players.Take<CompetitionPlayer>(details.Type.PlayersCount) select new Simple.SAMS.CompetitionEngine.Player { Id = p.Id, Rank = p.CompetitionRank }).ToArray<Simple.SAMS.CompetitionEngine.Player>()
        };
        
        return positioningEngine.Evaluate(positioningParameters);
    }

    public UpdatePlayerPositionInfo[] PositionPlayers(CompetitionDetails details)
    {
        CompetitionType competitionType = details.Type;
        MatchHeaderInfo[] orderedMatches = (from m in details.Matches
            orderby m.Position
            select m).Take<MatchHeaderInfo>((competitionType.PlayersCount / 2)).ToArray<MatchHeaderInfo>();
        List<UpdatePlayerPositionInfo> items = new List<UpdatePlayerPositionInfo>();
        if (details.Type.QualifyingPlayersCount > 0)
        {
            items.AddRange(this.GetQualifyingPlayersPositions(details));
        }
        items.AddRange(this.GetFinalPlayersPositions(details));
        return (from item in items
            where item.IsNotNull<UpdatePlayerPositionInfo>()
            select item).ToArray<UpdatePlayerPositionInfo>();
    }

    private static CompetitionPosition[] PositionQualifyingPlayers(CompetitionDetails details, CompetitionPlayer[] qualifyingPlayers)
    {
        var qualifyingPositioningEngine = new QualifyingPositioningEngine();
        var qualifyingPositioningParameters = new QualifyingPositionParameters
        {
            PlayersCount = details.Type.QualifyingPlayersCount,
            QualifyingCount = details.Type.QualifyingToFinalPlayersCount,
            Players = (from p in qualifyingPlayers.Take<CompetitionPlayer>(details.Type.QualifyingPlayersCount) select new Simple.SAMS.CompetitionEngine.Player { Id = p.Id, Rank = p.CompetitionRank }).ToArray<Simple.SAMS.CompetitionEngine.Player>()
        };
        
        return qualifyingPositioningEngine.Evaluate(qualifyingPositioningParameters);
    }


    public UpdatePlayerPositionInfo AddPlayer(PlayerInCompetition player, CompetitionSection section, CompetitionDetails details)
    {
        var result = default(UpdatePlayerPositionInfo);
        var maxRound = details.Matches.Where(m => m.Section == section).Max(m => m.Round);
        var match =
            details.Matches.FirstOrDefault(
                m => m.Section == section && m.Round == maxRound && (m.Player1.IsNull() || m.Player2.IsNull()));
        if (match.IsNotNull())
        {
            result = new UpdatePlayerPositionInfo();
            result.MatchId = match.Id;
            result.PlayerId = player.PlayerId;
            result.Position = match.Player1.IsNull() ? 0 : 1;
        }
        return result;
    }
}

