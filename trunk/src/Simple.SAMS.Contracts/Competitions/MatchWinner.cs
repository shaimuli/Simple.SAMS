﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public enum MatchWinner
    {
        [EnumMember]
        None,
        [EnumMember]
        Player1,
        [EnumMember]
        Player2
    }
}
