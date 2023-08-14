namespace GetMeThere.API.Repositories
{
    public interface ITaxiHubRepository
    {

        Task<bool> UpdateUserConnectionId(string username, string connectionId);

    }
}
