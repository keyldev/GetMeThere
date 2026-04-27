using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface IDispatchService
    {
        Task StartDispatchAsync(Guid orderId);
        Task<bool> AcceptOfferAsync(Guid orderId, Guid driverId);
        Task DeclineOfferAsync(Guid orderId, Guid driverId);
    }
}
