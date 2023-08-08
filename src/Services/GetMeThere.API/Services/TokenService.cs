using GetMeThere.API.Models;
using System.Security.Claims;

namespace GetMeThere.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAccessToken(IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }

        public JwtAuthResult GetTokens(User user, IEnumerable<Claim> claims, DateTime date)
        {
            throw new NotImplementedException();
        }
        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }
        private RefreshToken GenerateRefreshToken(Guid userId, DateTime date)
        {
            throw new NotImplementedException();
        }

    }
}
