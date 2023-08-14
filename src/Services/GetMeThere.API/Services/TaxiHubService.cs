using GetMeThere.API.Repositories;

namespace GetMeThere.API.Services
{
    public class TaxiHubService : ITaxiHubService
    {

        private readonly ITaxiHubRepository _taxiHubRepository;

        public TaxiHubService(ITaxiHubRepository hubRepository)
        {
            _taxiHubRepository = hubRepository;
        }
        public async Task UpdateUserConnectionId(string username, string connectionId)
        {
            await _taxiHubRepository.UpdateUserConnectionId(username, connectionId);
        }
    }
}
