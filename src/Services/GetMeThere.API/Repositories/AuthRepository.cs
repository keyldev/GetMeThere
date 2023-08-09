﻿using GetMeThere.API.Data;
using GetMeThere.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthRepository(IConfiguration configuration, ApplicationDbContext db)
        {
            _dbContext = db;
        }

        public User CreateUser(RegisterRequest request)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Login == request.Login || u.Login == request.Email);
            if (user == null)
            {
                User u = new User(request);

                _dbContext.Users.Add(u);
                _dbContext.SaveChanges();
                return u;
            }
            else
            {
                return null;
            }

        }
        public RefreshToken GetUserRefreshToken(string login)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Login == login);

            // исправить, если пользователь будет не null, а token null.
            if (user == null)
            {
                return null;

            }
            else
            {
                var token = _dbContext.RefreshTokens.FirstOrDefault(rt => rt.UserId == user.Id);
                return token;
            }
        }

        public void InsertUserRefreshToken(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Add(refreshToken);
            _dbContext.SaveChanges();

        }

        public bool IsUserExists(string login, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            return user != null;
        }
        public async Task<User> GetUser(string login, string password)
        {        
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
        }

        public async Task<bool> UpdateUserRefreshToken(Guid userId, RefreshToken refreshToken)
        {
            var userToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
            if (userToken != null)
            {
                userToken.TokenString = refreshToken.TokenString;

                userToken.ExpiryTime = refreshToken.ExpiryTime;
                
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
