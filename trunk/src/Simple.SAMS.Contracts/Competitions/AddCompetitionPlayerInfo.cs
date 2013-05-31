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
    public class AddCompetitionPlayerInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public string CompetitionReferenceId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionSection? Section { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionPlayerSource Source { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Player Player { get; set; }

    }
}
