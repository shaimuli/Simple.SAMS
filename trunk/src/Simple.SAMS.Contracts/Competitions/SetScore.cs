using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class SetScore
    {
        [DataMember(EmitDefaultValue = false)]
        public int Number { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Player1Points { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? BreakPoints { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Player2Points { get; set; }
    }
}
