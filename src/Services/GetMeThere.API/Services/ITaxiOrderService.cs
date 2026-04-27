using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public enum LifecycleResult
    {
        Ok,
        NotFound,
        NotOwned,
        InvalidState
    }

    public interface ITaxiOrderService
    {
        Task<TaxiOrder> CreateAsync(TaxiOrder order, Guid clientId);
        Task<LifecycleResult> CancelAsync(Guid orderId, Guid clientId);
        Task<LifecycleResult> MarkEnRouteAsync(Guid orderId, Guid driverId);
        Task<LifecycleResult> MarkArrivedAsync(Guid orderId, Guid driverId);
        Task<LifecycleResult> StartTripAsync(Guid orderId, Guid driverId);
        Task<LifecycleResult> CompleteTripAsync(Guid orderId, Guid driverId);
    }
}
