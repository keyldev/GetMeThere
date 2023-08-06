using GetMeThere.API.Models;
using System.Security.Claims;

namespace GetMeThere.API.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Creates JWT-AccessToken
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string GenerateAccessToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Creates two tokens: JWT & Refresh
        /// </summary>
        /// <returns></returns>
        public JwtAuthResult GenerateTokens();


    }
}
    