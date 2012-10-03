using Simple.SAMS.Contracts.Competitions;

namespace Simple.SAMS.Contracts.Users
{
    
    public interface IUsersRepository
    {
        UserInfo[] GetUsers();
    }
}
