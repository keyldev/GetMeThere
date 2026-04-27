using GetMeThere.API.Data;
using GetMeThere.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Repositories
{
    public class TaxiOrderRepository : ITaxiOrderRepository
    {
        private static readonly OrderStatus[] ActiveStatuses =
        {
            OrderStatus.Created,
            OrderStatus.Searching,
            OrderStatus.Assigned,
            OrderStatus.DriverEnRoute,
            OrderStatus.DriverArrived,
            OrderStatus.InProgress
        };

        private readonly ApplicationDbContext _dbContext;
        public TaxiOrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaxiOrder> CreateAsync(TaxiOrder order)
        {
            await _dbContext.TaxiOrders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public Task<TaxiOrder?> GetByIdAsync(Guid orderId)
        {
            return _dbContext.TaxiOrders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public Task<TaxiOrder?> GetActiveByClientAsync(Guid clientId)
        {
            return _dbContext.TaxiOrders
                .Where(o => o.ClientId == clientId && ActiveStatuses.Contains(o.Status))
                .OrderByDescending(o => o.PickupTime)
                .FirstOrDefaultAsync();
        }

        public Task<TaxiOrder?> GetActiveByDriverAsync(Guid driverId)
        {
            return _dbContext.TaxiOrders
                .Where(o => o.DriverId == driverId && ActiveStatuses.Contains(o.Status))
                .OrderByDescending(o => o.AssignedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _dbContext.TaxiOrders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false;
            order.Status = newStatus;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignDriverAsync(Guid orderId, Guid driverId)
        {
            var rows = await _dbContext.TaxiOrders
                .Where(o => o.OrderId == orderId && o.Status == OrderStatus.Searching && o.DriverId == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(o => o.DriverId, driverId)
                    .SetProperty(o => o.Status, OrderStatus.Assigned)
                    .SetProperty(o => o.AssignedAt, DateTime.UtcNow));
            return rows == 1;
        }

        public async Task<bool> SetArrivalAsync(Guid orderId, DateTime arrivedAt)
        {
            var order = await _dbContext.TaxiOrders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false;
            order.DriverArrivedAt = arrivedAt;
            order.Status = OrderStatus.DriverArrived;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetRouteAsync(Guid orderId, string polyline, DateTime estimatedArrival)
        {
            var order = await _dbContext.TaxiOrders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return false;
            order.RoutePolyline = polyline;
            order.EstimatedArrival = estimatedArrival;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
