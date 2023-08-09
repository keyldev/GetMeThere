using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface IAuthService
    {
        public Task<JwtAuthResult> Login(LoginRequest request);
        public JwtAuthResult Register(RegisterRequest request);

    }
}
