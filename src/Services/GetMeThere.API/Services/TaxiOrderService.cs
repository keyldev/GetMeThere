using GetMeThere.API.Hubs;
using GetMeThere.API.Models;
using GetMeThere.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Services
{
    public class TaxiOrderService : ITaxiOrderService
    {
        private readonly ITaxiOrderRepository _orders;
        private readonly ITaxiDriverRepository _drivers;
        private readonly IDispatchService _dispatch;
        private readonly IHubContext<TaxiHub> _hub;

        public TaxiOrderService(
            ITaxiOrderRepository orders,
            ITaxiDriverRepository drivers,
            IDispatchService dispatch,
            IHubContext<TaxiHub> hub)
        {
            _orders = orders;
            _drivers = drivers;
            _dispatch = dispatch;
            _hub = hub;
        }

        public async Task<TaxiOrder> CreateAsync(TaxiOrder order, Guid clientId)
        {
            order.OrderId = Guid.NewGuid();
            order.ClientId = clientId;
            order.Status = OrderStatus.Searching;
            order.PickupTime = DateTime.UtcNow;
            order.DriverId = null;

            var saved = await _orders.CreateAsync(order);
            await _dispatch.StartDispatchAsync(saved.OrderId);
            return saved;
        }

        public async Task<LifecycleResult> CancelAsync(Guid orderId, Guid clientId)
        {
            var order = await _orders.GetByIdAsync(orderId);
            if (order == null) return LifecycleResult.NotFound;
            if (order.ClientId != clientId) return LifecycleResult.NotOwned;
            if (!OrderStateMachine.CanTransition(order.Status, OrderStatus.Cancelled))
                return LifecycleResult.InvalidState;

            await _orders.UpdateStatusAsync(orderId, OrderStatus.Cancelled);
            if (order.DriverId is Guid driverId)
            {
                await _drivers.UpdateStatusAsync(driverId, DriverStatus.Available);
                await _hub.Clients.Group(Groups.Driver(driverId)).SendAsync("OrderCancelled", new { orderId });
            }
            return LifecycleResult.Ok;
        }

        public Task<LifecycleResult> MarkEnRouteAsync(Guid orderId, Guid driverId) =>
            DriverTransition(orderId, driverId, OrderStatus.DriverEnRoute, "DriverEnRoute");

        public async Task<LifecycleResult> MarkArrivedAsync(Guid orderId, Guid driverId)
        {
            var order = await _orders.GetByIdAsync(orderId);
            if (order == null) return LifecycleResult.NotFound;
            if (order.DriverId != driverId) return LifecycleResult.NotOwned;
            if (!OrderStateMachine.CanTransition(order.Status, OrderStatus.DriverArrived))
                return LifecycleResult.InvalidState;

            await _orders.SetArrivalAsync(orderId, DateTime.UtcNow);
            await _hub.Clients.Group(Groups.Client(order.ClientId)).SendAsync("DriverArrived", new { orderId });
            return LifecycleResult.Ok;
        }

        public Task<LifecycleResult> StartTripAsync(Guid orderId, Guid driverId) =>
            DriverTransition(orderId, driverId, OrderStatus.InProgress, "TripStarted");

        public async Task<LifecycleResult> CompleteTripAsync(Guid orderId, Guid driverId)
        {
            var order = await _orders.GetByIdAsync(orderId);
            if (order == null) return LifecycleResult.NotFound;
            if (order.DriverId != driverId) return LifecycleResult.NotOwned;
            if (!OrderStateMachine.CanTransition(order.Status, OrderStatus.Completed))
                return LifecycleResult.InvalidState;

            await _orders.UpdateStatusAsync(orderId, OrderStatus.Completed);
            await _drivers.UpdateStatusAsync(driverId, DriverStatus.Available);
            await _hub.Clients.Group(Groups.Client(order.ClientId)).SendAsync("TripCompleted", new { orderId });
            return LifecycleResult.Ok;
        }

        private async Task<LifecycleResult> DriverTransition(Guid orderId, Guid driverId, OrderStatus next, string clientEvent)
        {
            var order = await _orders.GetByIdAsync(orderId);
            if (order == null) return LifecycleResult.NotFound;
            if (order.DriverId != driverId) return LifecycleResult.NotOwned;
            if (!OrderStateMachine.CanTransition(order.Status, next))
                return LifecycleResult.InvalidState;

            await _orders.UpdateStatusAsync(orderId, next);
            await _hub.Clients.Group(Groups.Client(order.ClientId)).SendAsync(clientEvent, new { orderId });
            return LifecycleResult.Ok;
        }
    }
}
