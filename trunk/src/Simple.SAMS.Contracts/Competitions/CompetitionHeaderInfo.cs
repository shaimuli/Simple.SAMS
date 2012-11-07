using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionHeaderInfo
    {

        [DataMember(EmitDefaultValue = false)]
        public CompetitionStatus Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ReferenceId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionType Type { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime StartTime { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public DateTime? EndTime { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public DateTime LastModified { get; set; }

    }
}
