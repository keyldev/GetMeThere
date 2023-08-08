using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface IUserRepository
    {

        //?? or 
        public bool IsUserExists(string username, string password);
        public RefreshToken GetRefreshToken(Guid userId);
    }
}
