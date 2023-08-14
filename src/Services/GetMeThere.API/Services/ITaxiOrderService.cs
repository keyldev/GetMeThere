using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface ITaxiOrderService
    {
        Task SendNewOrderNotification(TaxiOrder order);
    }
}
