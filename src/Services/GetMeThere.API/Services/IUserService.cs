using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Services
{
    public interface IUserService
    {
        // user or UserDto
        Task<UserDto> GetUserInfo(string username); // or userid?
        Task<UserDto> UpdateUserInfo(UserDto userDto);
        Task<bool> DeleteUserAccount(UserDto user);

    }
}
