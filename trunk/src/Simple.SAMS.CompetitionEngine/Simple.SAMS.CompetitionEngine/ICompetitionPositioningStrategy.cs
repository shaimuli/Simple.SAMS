using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.CompetitionEngine
{
    public interface ICompetitionPositioningStrategy
    {
        CompetitionPosition[] PositionPlayers(CompetitionPositioningParameters parameters);
    }
}
