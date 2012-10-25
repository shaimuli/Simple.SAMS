using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Positioning;

namespace Simple.SAMS.Contracts.Competitions
{
    public interface IPositioningEngineFactory
    {
        IPositioningEngine Create(CompetitionMethod method);
    }
}
