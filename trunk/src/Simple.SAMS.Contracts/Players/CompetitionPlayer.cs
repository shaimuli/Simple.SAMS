using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Players
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionPlayer : Player
    {
        [DataMember(EmitDefaultValue = false)]
        public int CompetitionRank { get; set; }
    }
}
