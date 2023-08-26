using GetMeThere.API.Hubs;
using GetMeThere.API.Models;
using GetMeThere.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Services
{
    public class TaxiOrderService : ITaxiOrderService
    {
        private readonly IHubContext<TaxiHub> _taxiHubContext;
        private readonly ITaxiOrderRepository _taxiOrderRepository;
        public TaxiOrderService(IHubContext<TaxiHub> hubContext, ITaxiOrderRepository taxiOrderRepository)
        {
            _taxiHubContext = hubContext;
            _taxiOrderRepository = taxiOrderRepository;
        }
        public async Task SendNewOrderNotification(TaxiOrder order)
        {
            await _taxiHubContext.Clients.All.SendAsync("NewOrder", order);
        }

    }
}
