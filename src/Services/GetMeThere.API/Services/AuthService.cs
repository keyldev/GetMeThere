using GetMeThere.API.Models;
using GetMeThere.API.Repositories;

namespace GetMeThere.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        public AuthService(IConfiguration configuration, IAuthRepository authRepository)
        {
            _configuration = configuration;
            _authRepository = authRepository;
        }

        public JwtAuthResult Login(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
