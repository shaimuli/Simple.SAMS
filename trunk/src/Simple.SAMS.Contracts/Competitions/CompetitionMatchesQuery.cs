using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionMatchesQuery
    {
        [DataMember(EmitDefaultValue = false)]
        public int CompetitionId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? Date { get; set; }
    }
}
