using GetMeThere.API.Configuration;
using GetMeThere.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GetMeThere.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwt;

        public TokenService(JwtOptions jwt)
        {
            _jwt = jwt;
        }

        public string GetAccessToken(IEnumerable<Claim> claims) => GenerateAccessToken(claims);

        public RefreshToken GetRefreshToken(Guid userId, DateTime date) => GenerateRefreshToken(userId, date);

        public JwtAuthResult GetTokens(User user, IEnumerable<Claim> claims, DateTime date) =>
            new JwtAuthResult
            {
                AccessToken = GenerateAccessToken(claims),
                RefreshToken = GenerateRefreshToken(user.Id, date)
            };

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                expires: DateTime.UtcNow.AddMinutes(30),
                claims: claims,
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static RefreshToken GenerateRefreshToken(Guid userId, DateTime date)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new RefreshToken
            {
                UserId = userId,
                ExpiryTime = date.AddMonths(3),
                TokenString = Convert.ToBase64String(randomNumber)
            };
        }
    }
}
