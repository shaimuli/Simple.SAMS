using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts.Players
{
    [DataContract(Namespace = Namespaces.Data)]
    public class Player : Entity
    {
        [DataMember(EmitDefaultValue = false)]
        public int? AverageScore { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public int? AccumulatedScore { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string IdNumber { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string LocalLastName { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string LocalFirstName { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string EnglishLastName { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string EnglishFirstName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Phone { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool IsFemale { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public DateTime? BirthDate { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string IPIN { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public string Country { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? NationalRank { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int? EuropeInternationalRank { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int? YouthInternationalRank { get; set; }


    }
}
