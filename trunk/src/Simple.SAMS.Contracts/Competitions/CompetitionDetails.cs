using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionDetails : CompetitionHeaderInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public bool CanAddToQualifying { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public bool CanAddToFinal { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string MainRefereeName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string MainRefereePhone { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string Site { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string SitePhone { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionPlayer[] Players { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MatchHeaderInfo[] Matches { get; set; }
    }
}
