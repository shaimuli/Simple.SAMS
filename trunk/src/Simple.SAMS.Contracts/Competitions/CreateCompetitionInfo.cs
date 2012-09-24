﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class CreateCompetitionInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int TypeId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime StartTime { get; set; }    

        [DataMember(EmitDefaultValue = false)]
        public string PlayersFileUrl { get; set; }
    }
}