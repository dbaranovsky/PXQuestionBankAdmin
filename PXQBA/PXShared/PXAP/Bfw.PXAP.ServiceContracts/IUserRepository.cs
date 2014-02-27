using System.Linq;
using Bfw.PXAP.Models.Domain;

namespace Bfw.PXAP.ServiceContracts
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsers();
        User GetUser(string username);
        void SaveUser(User user, bool createUser = false);
        void DeleteUser(string userName);
    }
}
