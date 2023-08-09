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

        public async Task<JwtAuthResult> Login(LoginRequest request)
        {
            var user = await _authRepository.GetUser(login: request.Username, password: request.Password);
            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Username)
                };
                var accessToken = _tokenService.GetAccessToken(claims);
                var refreshToken = _authRepository.GetUserRefreshToken(request.Username);
                

                // sugar. #changeme
                if(refreshToken is not null && refreshToken.ExpiryTime < DateTime.UtcNow)
                {
                    refreshToken = _tokenService.GetRefreshToken(user.Id, DateTime.UtcNow);
                    _authRepository.UpdateUserRefreshToken(user.Id, refreshToken);
                }
                else
                {
                    refreshToken = _tokenService.GetRefreshToken(user.Id, DateTime.UtcNow);
                    _authRepository.InsertUserRefreshToken(refreshToken);
                }

                
                return new JwtAuthResult
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = 123182785 // fix that
                };
            }
            else
            {
                return null;
            }
        }

        public JwtAuthResult Register(RegisterRequest request)
        {
            var isUserExists = _authRepository.IsUserExists(login: request.Login, password: request.Password);
            if (isUserExists)
            {
                return null;
            }
            else
            {
                var user = _authRepository.CreateUser(request);
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name)
                };
                
                var tokens = _tokenService.GetTokens(user, claims, DateTime.UtcNow);
                _authRepository.InsertUserRefreshToken(tokens.RefreshToken);

                return tokens;
            }
        }
    }
}
