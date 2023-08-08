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
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAccessToken(IEnumerable<Claim> claims)
        {
            return GenerateAccessToken(claims);
        }

        public JwtAuthResult GetTokens(User user, IEnumerable<Claim> claims, DateTime date)
        {
            return new JwtAuthResult
            {
                AccessToken = GenerateAccessToken(claims),
                RefreshToken = GenerateRefreshToken(user.Id, date)
            };
        }
        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt")["Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt")["Issuer"],
                audience: _configuration.GetSection("Jwt")["Audience"],
                expires: DateTime.UtcNow.AddMinutes(30),
                claims: claims,
                signingCredentials: signingCredentials
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }
        private RefreshToken GenerateRefreshToken(Guid userId, DateTime date)
        {
            var randomNumber = new byte[32];
            using var numberGenerator = RandomNumberGenerator.Create();
            numberGenerator.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);

            return new RefreshToken
            {
                UserId = userId,
                ExpiryTime = date.AddMonths(3),
                TokenString = token
            };
        }

    }
}
