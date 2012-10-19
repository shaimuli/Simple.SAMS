using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class MatchPlayer
    {
        [DataMember(EmitDefaultValue = false)]
        public int Id { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string IdNumber { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LocalFirstName { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string LocalLastName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string EnglishFirstName { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string EnglishLastName { get; set; }


    }
}
