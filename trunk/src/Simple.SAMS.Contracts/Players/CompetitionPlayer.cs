using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts.Players
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionPlayer : Player
    {
        [DataMember(EmitDefaultValue = false)]
        public int? Position { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Points { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string CompetitionReferenceId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public CompetitionPlayerStatus Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Replaceable { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int CompetitionRank { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionPlayerSource Source { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public CompetitionSection Section { get; set; }

    }
}
