using GetMeThere.API.Data;
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
        public User GetUser(string login, string password)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public bool UpdateUserRefreshToken(Guid userId, RefreshToken refreshToken)
        {
            var userToken = _dbContext.RefreshTokens.SingleOrDefault(rt => rt.UserId == userId);
            if (userToken != null)
            {
                refreshToken.TokenId = userToken.TokenId;
                
                _dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
