using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CompetitionType : Entity
    {
        [DataMember(EmitDefaultValue = false)]
        public Ranking Ranking { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionMethod Method { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int PlayersCount { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int WildcardPlayersCount { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int QualifyingPlayersCount { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int PairsCount { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int WildcardPairsCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int QualifyingPairsCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool HasConsolation { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int QualifyingWildcardPlayersCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int QualifyingWildcardPairsCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int QualifyingToFinalPlayersCount { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int QualifyingToFinalPaisCount { get; set; }

    }
}
