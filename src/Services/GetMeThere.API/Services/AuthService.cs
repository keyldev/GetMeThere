using GetMeThere.API.Models;
using GetMeThere.API.Repositories;
using System.Security.Claims;

namespace GetMeThere.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        public AuthService(IConfiguration configuration, IAuthRepository authRepository, ITokenService tokenService)
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        public JwtAuthResult Login(LoginRequest request)
        {
            var isUserExists = _authRepository.IsUserExists(login: request.Username, password: request.Password);
            if(isUserExists)
            {
                var refreshToken = _authRepository.GetUserRefreshToken(request.Username);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Username)
                };
                var accessToken = _tokenService.GetAccessToken(claims);
                return new JwtAuthResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = 198527245
                };
            }
            else
            {
                return null;
            }
        }
    }
}
