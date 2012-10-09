using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts
{
    public class DefaultMatchProvisioningEngineFactory : IMatchProvisioningEngineFactory
    {
        public IMatchProvisioningEngine Create(CompetitionType competitionType)
        {
            if (competitionType.Method == CompetitionMethod.Knockout)
            {
                return Build.New<KnockoutMatchProvisioningEngine>();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
