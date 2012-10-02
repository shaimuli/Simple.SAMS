using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class MatchInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime StartTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player1 { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player2 { get; set; }


    }
}
