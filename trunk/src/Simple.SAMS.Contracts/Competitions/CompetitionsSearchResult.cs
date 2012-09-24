using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionsSearchResult
    {

        [DataMember(EmitDefaultValue = false)]
        public int TotalCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionHeaderInfo[] Items { get; set; }
    }
}
