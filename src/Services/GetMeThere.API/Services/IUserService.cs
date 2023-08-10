using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface IUserService
    {
        // user or UserDto
        void GetUserInfo(string username); // or userid?

    }
}
