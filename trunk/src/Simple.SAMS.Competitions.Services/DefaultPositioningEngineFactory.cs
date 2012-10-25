using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Competitions.Services
{
    public class DefaultPositioningEngineFactory : IPositioningEngineFactory
    {
        public Contracts.Positioning.IPositioningEngine Create(CompetitionMethod method)
        {
            if (method != CompetitionMethod.Knockout)
            {
                throw new NotImplementedException();
            }

            return Build.New<KnockoutPositioningEngine>();
        }
    }
}
