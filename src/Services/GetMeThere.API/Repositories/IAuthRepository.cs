using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> IsLoginTaken(string login);
        Task<User?> GetUserByLogin(string login);
        Task<User?> GetUserById(Guid userId);
        Task<User> CreateUser(User user);

        Task<RefreshToken?> GetUserRefreshToken(Guid userId);
        Task<RefreshToken?> GetRefreshTokenByString(string tokenString);
        Task<bool> UpdateUserRefreshToken(Guid userId, RefreshToken refreshToken);
        Task InsertUserRefreshToken(RefreshToken refreshToken);
    }
}
