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
        public string GetAccessToken(IEnumerable<Claim> claims);

        /// <summary>
        /// Generate's and returns the access token and the refresh token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claims"></param>
        /// <param name="date"></param>
        /// <returns>Refresh & Access tokens</returns>
        public JwtAuthResult GetTokens(User user, IEnumerable<Claim> claims, DateTime date);
        public RefreshToken GetRefreshToken(Guid userId, DateTime date);
        /// <summary>
        /// Method for generating refresh token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="date">Today's date</param>
        /// <returns></returns>
        //private protected RefreshToken GenerateRefreshToken(Guid userId, DateTime date);
    }
}
    