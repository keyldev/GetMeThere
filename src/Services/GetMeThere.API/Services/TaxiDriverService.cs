using GetMeThere.API.Hubs;
using GetMeThere.API.Models.DTO;
using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Services
{
    public class TaxiDriverService : ITaxiDriverService
    {
        private readonly IHubContext<TaxiHub> taxiHubContext;
        public TaxiDriverService(IHubContext<TaxiHub> taxiHubContext)
        {
            this.taxiHubContext = taxiHubContext;
        }

        public Task<List<TaxiDriverDto>> GetAllDriversAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SendOrderAccepted(OrderAcceptDto orderAcceptDto)
        {
            // for test
            await taxiHubContext.Clients.Client(orderAcceptDto.OrderId).SendAsync("RecieveOrder");
        }
    }
}
