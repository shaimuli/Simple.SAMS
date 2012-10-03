using System.Runtime.Serialization;

namespace Simple.SAMS.Contracts.Users
{
    [DataContract(Namespace = Namespaces.Data)]
    public class UserInfo
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}
