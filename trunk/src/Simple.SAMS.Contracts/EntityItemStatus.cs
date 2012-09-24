using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts
{
    [DataContract(Namespace = Namespaces.Data)]
    public enum EntityItemStatus
    {
        [EnumMember]
        Active,
        [EnumMember]
        Archived,
        [EnumMember]
        Deleted
    }
}
