using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface IUserRepository
    {

        Task<User> GetUserInfo(string username);
    }
}
