﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class MatchStartTimeUpdateInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public int MatchId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime StartTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public StartTimeType StartTimeType { get; set; }
    }
}
