using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        public RefreshToken GetRefreshToken(Guid userId)
        {
            throw new NotImplementedException();
        }

        public bool IsUserExists(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
