using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface IAuthService
    {
        public JwtAuthResult Login(LoginRequest request);
    }
}
