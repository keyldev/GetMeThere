using GetMeThere.API.Data;
using GetMeThere.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthRepository(ApplicationDbContext db)
        {
            _dbContext = db;
        }

        public async Task<bool> IsLoginTaken(string login)
        {
            return await _dbContext.Users.AnyAsync(u => u.Login == login);
        }

        public async Task<User?> GetUserByLogin(string login)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> CreateUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<RefreshToken?> GetUserRefreshToken(Guid userId)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
        }

        public async Task<RefreshToken?> GetRefreshTokenByString(string tokenString)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenString == tokenString);
        }

        public async Task InsertUserRefreshToken(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserRefreshToken(Guid userId, RefreshToken refreshToken)
        {
            var userToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
            if (userToken == null)
                return false;

            userToken.TokenString = refreshToken.TokenString;
            userToken.ExpiryTime = refreshToken.ExpiryTime;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
