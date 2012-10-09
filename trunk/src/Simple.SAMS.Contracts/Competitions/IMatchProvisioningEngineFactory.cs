using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    public interface IMatchProvisioningEngineFactory
    {
        IMatchProvisioningEngine Create(CompetitionType competitionType);
    }
}
