using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public enum CompetitionStatus
    {
        [EnumMember]
        Created,
        [EnumMember]
        MatchesCreated,
        [EnumMember]
        Positioned,
        [EnumMember]
        Started,
        [EnumMember]
        Finished
    }
}
