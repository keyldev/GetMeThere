using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface IUserService
    {
        // user or UserDto
        User CreateUser(User user);

    }
}
