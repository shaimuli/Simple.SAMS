using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class UpdatePlayerPositionInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public int PlayerId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int MatchId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Position { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SlotType? SlotType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? SlotPosition { get; set; }

    }
}
