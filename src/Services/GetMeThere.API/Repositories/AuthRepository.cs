using GetMeThere.API.Data;

namespace GetMeThere.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthRepository(IConfiguration configuration, ApplicationDbContext db)
        {
            _dbContext = db;
        }

        public bool IsUserExists(string login, string password)
        {
            throw new NotImplementedException();
        }
    }
}
