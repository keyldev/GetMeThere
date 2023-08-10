using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using GetMeThere.API.Repositories;

namespace GetMeThere.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserDto GetUserInfo(string username)
        {
            throw new NotImplementedException();
        }
    }
}
