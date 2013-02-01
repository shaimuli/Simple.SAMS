using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public enum CompetitionSection
    {
        [EnumMember]
        None,
        [EnumMember]
        Final = 1,
        [EnumMember]
        Qualifying = 2,
        [EnumMember]
        Consolation = 3,
        [EnumMember]
        PairsQualifying = 4,
        [EnumMember]
        PairsFinal = 5
    }
}
