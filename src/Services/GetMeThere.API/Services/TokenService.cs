using GetMeThere.API.Models;
using System.Security.Claims;

namespace GetMeThere.API.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }

        public JwtAuthResult GenerateTokens(User user, IEnumerable<Claim> claims, DateTime date)
        {
            throw new NotImplementedException();
        }
        RefreshToken GenerateRefreshToken(Guid userId, DateTime date)
        {
            throw new NotImplementedException();
        }

    }
}
