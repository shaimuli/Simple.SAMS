using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionSearchQuery
    {
        [DataMember(EmitDefaultValue = false)]
        public int StartIndex { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int PageSize { get; set; }
    }
}
