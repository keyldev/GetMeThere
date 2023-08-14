using GetMeThere.API.Hubs;
using GetMeThere.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Services
{
    public class TaxiOrderService : ITaxiOrderService
    {
        private readonly IHubContext<TaxiHub> _taxiHubContext;
        public TaxiOrderService(IHubContext<TaxiHub> hubContext)
        {
            _taxiHubContext = hubContext;
        }
        public async Task SendNewOrderNotification(TaxiOrder order)
        {
            await _taxiHubContext.Clients.All.SendAsync("NewOrder", order);
        }

    }
}
