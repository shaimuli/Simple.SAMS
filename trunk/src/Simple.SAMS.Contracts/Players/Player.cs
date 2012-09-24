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
    public class Player : Entity
    {
        [DataMember(EmitDefaultValue = false)]
        public string IdNumber { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Rank { get; set; }
    }
}
