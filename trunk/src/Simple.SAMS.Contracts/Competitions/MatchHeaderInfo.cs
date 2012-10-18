﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class MatchHeaderInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public CompetitionSection Section { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public StartTimeType StartTimeType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MatchStatus Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? Position { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int Round { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? StartTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player1 { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player2 { get; set; }


    }
}