using GetMeThere.API.Models;
using GetMeThere.API.Repositories;

namespace GetMeThere.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public JwtAuthResult Login(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
