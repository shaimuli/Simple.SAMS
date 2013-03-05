﻿using System.Collections.Generic;
using System.Linq;
using Simple;
using Simple.SAMS.CompetitionEngine;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;
using Simple.SAMS.Contracts.Positioning;

public class KnockoutPositioningEngine : IPositioningEngine
{

    private UpdatePlayerPositionInfo[] GetFinalPlayersPositions(CompetitionDetails details)
    {
        var items = Enumerable.Empty<UpdatePlayerPositionInfo>();
        var orderedMatches = (from m in details.Matches
            where m.Section == CompetitionSection.Final
            orderby m.Position
            select m).Take<MatchHeaderInfo>(details.Type.PlayersCount / 2).ToArray();

        var finalPlayers = (from p in details.Players
            where p.Section == CompetitionSection.Final
            select p).ToArray<CompetitionPlayer>();
        if (details.Type.QualifyingPlayersCount > 0)
        {
            finalPlayers = finalPlayers.Take(details.Type.PlayersCount - details.Type.QualifyingToFinalPlayersCount).ToArray();
        }
        if (finalPlayers.Length > 0)
        {
            var finalPositions = PositionFinalPlayersInCompetition(details, finalPlayers);
            items = GetUpdatePlayerPositionInfos(finalPositions, orderedMatches);
        }
        return items.ToArray<UpdatePlayerPositionInfo>();
    }

    private UpdatePlayerPositionInfo[] GetQualifyingPlayersPositions(CompetitionDetails details)
    {
        var items = Enumerable.Empty<UpdatePlayerPositionInfo>();
        var orderedMatches = (from m in details.Matches
            where m.Section == CompetitionSection.Qualifying
            orderby m.Position
            select m).Take<MatchHeaderInfo>((details.Type.QualifyingPlayersCount / 2)).ToArray<MatchHeaderInfo>();
        var qualifyingPlayers = (from p in details.Players
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
        var matchIndex = 0;
        var positionIndex = 0;
        var results =  qualifyingPlayersPositions.Select(position => {
            var result = default(UpdatePlayerPositionInfo);
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

        return results;
    }

    private CompetitionPosition[] PositionFinalPlayersInCompetition(CompetitionDetails details, CompetitionPlayer[] players)
    {
        var positioningEngine = new FinalPositioningEngine();
        var positioningParameters = new FinalPositioningParameters
        {
            PlayersCount = details.Type.PlayersCount,
            RankedPlayersCount = details.Type.FinalRankedPlayersCount,
            Players = (from p in players select new Simple.SAMS.CompetitionEngine.Player { Id = p.Id, Rank = p.CompetitionRank }).ToArray<Simple.SAMS.CompetitionEngine.Player>()
        };
        
        return positioningEngine.Evaluate(positioningParameters);
    }

    public UpdatePlayerPositionInfo[] PositionPlayers(CompetitionDetails details, CompetitionSection section)
    {
        var items = new List<UpdatePlayerPositionInfo>();
        if (details.Type.QualifyingPlayersCount > 0 && section == CompetitionSection.Qualifying)
        {
            items.AddRange(this.GetQualifyingPlayersPositions(details));
        }
        if (section == CompetitionSection.Final)
        {
            items.AddRange(this.GetFinalPlayersPositions(details));
        }
        return (from item in items
            where item.IsNotNull<UpdatePlayerPositionInfo>()
            select item).ToArray<UpdatePlayerPositionInfo>();
    }

    private CompetitionPosition[] PositionQualifyingPlayers(CompetitionDetails details, CompetitionPlayer[] qualifyingPlayers)
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


    public UpdatePlayerPositionInfo AddPlayerToSection(int playerId, CompetitionSection section, CompetitionDetails details)
    {
        var result = default(UpdatePlayerPositionInfo);
        // rounds are in descending order, so first will be the max
        var maxRound = details.Matches.Where(m => m.Section == section).Max(m => m.Round);
        var match =
            details.Matches.FirstOrDefault(
                m => m.Section == section && m.Round == 1 && (m.Player1.IsNull() || m.Player2.IsNull()));
        if (match.IsNotNull())
        {
            result = new UpdatePlayerPositionInfo();
            result.MatchId = match.Id;
            result.PlayerId = playerId;
            result.Position = match.Player1.IsNull() ? 0 : 1;
        }
        return result;
    }
}

