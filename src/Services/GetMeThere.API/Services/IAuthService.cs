using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface IAuthService
    {
        public Task<JwtAuthResult> Login(LoginRequest request);
        public Task<JwtAuthResult> Register(RegisterRequest request);

    }
}
