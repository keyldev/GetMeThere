using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using GetMeThere.API.Repositories;
using System.Security.Claims;

namespace GetMeThere.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IAuthRepository authRepository, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResultDto?> Login(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return null;

            var user = await _authRepository.GetUserByLogin(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResultDto?> Register(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Password))
                return null;

            if (await _authRepository.IsLoginTaken(request.Login))
                return null;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = request.Login,
                Name = request.Name ?? "",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            user = await _authRepository.CreateUser(user);

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResultDto?> Refresh(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var stored = await _authRepository.GetRefreshTokenByString(refreshToken);
            if (stored == null || stored.ExpiryTime < DateTime.UtcNow)
                return null;

            var user = await _authRepository.GetUserById(stored.UserId);
            if (user == null)
                return null;

            return await IssueTokensAsync(user);
        }

        private async Task<AuthResultDto> IssueTokensAsync(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var accessToken = _tokenService.GetAccessToken(claims);
            var newRefresh = _tokenService.GetRefreshToken(user.Id, DateTime.UtcNow);

            var existing = await _authRepository.GetUserRefreshToken(user.Id);
            if (existing != null)
                await _authRepository.UpdateUserRefreshToken(user.Id, newRefresh);
            else
                await _authRepository.InsertUserRefreshToken(newRefresh);

            return new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefresh.TokenString
            };
        }
    }
}
