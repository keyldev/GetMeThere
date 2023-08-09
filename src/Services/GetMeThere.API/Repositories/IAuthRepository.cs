using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface IAuthRepository
    {

        Task<bool> IsUserExists(string login, string password);
        Task<User> GetUser(string login, string password);
        RefreshToken GetUserRefreshToken(string login);
        
        Task<bool> UpdateUserRefreshToken(Guid userId, RefreshToken refreshToken);
        Task InsertUserRefreshToken(RefreshToken refreshToken);

        Task<User> CreateUser(RegisterRequest request);

    }
}
