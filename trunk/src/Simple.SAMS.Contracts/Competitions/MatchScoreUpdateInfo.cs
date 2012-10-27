using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class MatchScoreUpdateInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public int MatchId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GameScoreUpdateInfo CurrentGameScore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SetScore[] SetScores { get; set; }

    }
}
