using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Services
{
    public interface IUserService
    {
        // user or UserDto
        UserDto GetUserInfo(string username); // or userid?

    }
}
