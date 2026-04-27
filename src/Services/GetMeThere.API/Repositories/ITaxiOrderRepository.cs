using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface ITaxiOrderRepository
    {
        Task<TaxiOrder> CreateAsync(TaxiOrder order);
        Task<TaxiOrder?> GetByIdAsync(Guid orderId);
        Task<TaxiOrder?> GetActiveByClientAsync(Guid clientId);
        Task<TaxiOrder?> GetActiveByDriverAsync(Guid driverId);
        Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus newStatus);
        Task<bool> AssignDriverAsync(Guid orderId, Guid driverId);
        Task<bool> SetArrivalAsync(Guid orderId, DateTime arrivedAt);
        Task<bool> SetRouteAsync(Guid orderId, string polyline, DateTime estimatedArrival);
    }
}
