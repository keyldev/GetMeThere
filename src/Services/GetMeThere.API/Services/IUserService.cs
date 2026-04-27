using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserInfo(string username);
        Task<UserDto> UpdateUserInfo(UserDto userDto);
        Task<bool> DeleteUserAccount(UserDto user);
    }
}
