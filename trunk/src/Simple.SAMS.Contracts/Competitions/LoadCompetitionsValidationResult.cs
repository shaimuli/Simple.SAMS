using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts;

namespace Simple.SAMS.Competitions.Services
{
    [DataContract(Namespace = Namespaces.Data)]
    public enum LoadCompetitionsValidationResult
    {
        [EnumMember]
        Valid,
        [EnumMember]
        InvalidCompetitionType,
        [EnumMember]
        InvalidFile
    }
}
