using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts.Positioning
{
    public interface IPositioningEngine
    {
        UpdatePlayerPositionInfo[] PositionPlayers(CompetitionDetails details);
        UpdatePlayerPositionInfo AddPlayerToSection(int playerId, CompetitionSection section, CompetitionDetails details);
    }
}
