using GetMeThere.API.Data;
using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> DeleteUserAccount(UserDto user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserInfo(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == username);
        }
        
    }
}
