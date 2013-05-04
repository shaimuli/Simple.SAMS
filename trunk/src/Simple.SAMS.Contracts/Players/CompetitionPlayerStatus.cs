using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Players
{
    [DataContract(Namespace = Namespaces.Data)]
    public enum CompetitionPlayerStatus
    {
        [EnumMember]
        Active,

        [EnumMember]
        Retired,

        [EnumMember]
        Disqualified,

        [EnumMember]
        Removed
    }
}
