using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface IAuthRepository
    {

        bool IsUserExists(string login, string password);
        Task<User> GetUser(string login, string password);
        RefreshToken GetUserRefreshToken(string login);
        
        Task<bool> UpdateUserRefreshToken(Guid userId, RefreshToken refreshToken);
        void InsertUserRefreshToken(RefreshToken refreshToken);

        User CreateUser(RegisterRequest request);

    }
}
