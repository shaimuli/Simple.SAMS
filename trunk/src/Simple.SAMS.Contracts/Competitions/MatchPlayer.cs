using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class MatchPlayer
    {
        [DataMember(EmitDefaultValue = false)]
        public int PlayerId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PlayerName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int CompetitionRank { get; set; }

    }
}
