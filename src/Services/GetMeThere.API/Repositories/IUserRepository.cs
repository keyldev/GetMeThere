using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Repositories
{
    public interface IUserRepository
    {

        Task<User> GetUserInfo(string username);
        Task<bool> DeleteUserAccount(UserDto user);
    }
}
