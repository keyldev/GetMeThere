namespace GetMeThere.API.Repositories
{
    public interface IAuthRepository
    {

        bool IsUserExists(string login, string password);

    }
}
