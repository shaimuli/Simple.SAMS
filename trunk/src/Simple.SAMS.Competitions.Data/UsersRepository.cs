using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Users;
using Simple.Utilities;

namespace Simple.SAMS.Competitions.Data
{
    public class UsersRepository : DataRepositoryBase<CompetitionsDataContext>, IUsersRepository
    {
        public UserInfo[] GetUsers()
        {
            var users = new List<UserInfo>();
            UseDataContext(
                dataContext =>
                    {
                        foreach(var dataUser in dataContext.Users)
                        {
                            var user = new UserInfo();
                            user.Username = dataUser.UserName;
                            user.IsActive = dataUser.IsActive.GetValueOrDefault();
                            users.Add(user);
                        }
                    });
            return users.ToArray();
        }
    }
}
