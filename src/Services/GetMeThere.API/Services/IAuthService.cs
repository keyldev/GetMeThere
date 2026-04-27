using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto?> Login(LoginRequest request);
        Task<AuthResultDto?> Register(RegisterRequest request);
        Task<AuthResultDto?> Refresh(string refreshToken);
    }
}
