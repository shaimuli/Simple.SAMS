using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class GameScoreUpdateInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public int ServingPlayer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Player1Points { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Player2Points { get; set; }
    }
}
