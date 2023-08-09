using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface IAuthRepository
    {

        bool IsUserExists(string login, string password);
        RefreshToken GetUserRefreshToken(string login);

    }
}
