using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Services
{
    public interface IAuthService
    {
        public Task<AuthResultDto> Login(LoginRequest request);
        public Task<AuthResultDto> Register(RegisterRequest request);

    }
}
