using GetMeThere.API.Hubs;
using Microsoft.AspNet.SignalR;

namespace GetMeThere.API.Services
{
    public class TaxiOrderService : ITaxiOrderService
    {
        private readonly IHubContext<TaxiHub> _taxiHubContext;
        public TaxiOrderService(IHubContext<TaxiHub> hubContext)
        {
            _taxiHubContext = hubContext;
        }

        //await _taxiHubContext.Clients.All.SendAsync("NewOrder", order);
    }
}
