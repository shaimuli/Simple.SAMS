using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class Entity : IEntity
    {
        [DataMember(EmitDefaultValue = false)]
        public int Id
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public DateTime Created
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public DateTime Updated
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public EntityItemStatus ItemStatus
        {
            get;
            set;
        }
    }
}
