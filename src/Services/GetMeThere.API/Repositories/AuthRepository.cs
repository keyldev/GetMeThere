﻿using GetMeThere.API.Data;
using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthRepository(IConfiguration configuration, ApplicationDbContext db)
        {
            _dbContext = db;
        }

        public RefreshToken GetUserRefreshToken(string login)
        {
            // tests
            return new RefreshToken()
            {
                TokenString = "asdbaksbdkj123",
                UserId = Guid.NewGuid(),
                ExpiryTime = DateTime.UtcNow.AddDays(5),
            };
        }

        public bool IsUserExists(string login, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            return user != null;
        }
    }
}
