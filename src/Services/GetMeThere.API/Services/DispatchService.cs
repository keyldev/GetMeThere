using System.Collections.Concurrent;
using GetMeThere.API.Hubs;
using GetMeThere.API.Models;
using GetMeThere.API.Notifications;
using GetMeThere.API.Repositories;
using GetMeThere.API.Services.Routing;
using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Services
{
    public class DispatchService : IDispatchService
    {
        private const double SearchRadiusKm = 5.0;
        private const int CandidateLimit = 10;
        private const int OfferTopN = 3;
        private static readonly TimeSpan OfferTimeout = TimeSpan.FromSeconds(10);

        private readonly IServiceProvider _sp;
        private readonly IHubContext<TaxiHub> _hub;
        private readonly IOsrmClient _osrm;
        private readonly INotificationDispatcher _notifications;
        private readonly ILogger<DispatchService> _logger;

        // Per-order offer state used to short-circuit waits when driver accepts/declines.
        private readonly ConcurrentDictionary<Guid, OfferState> _offers = new();

        public DispatchService(
            IServiceProvider sp,
            IHubContext<TaxiHub> hub,
            IOsrmClient osrm,
            INotificationDispatcher notifications,
            ILogger<DispatchService> logger)
        {
            _sp = sp;
            _hub = hub;
            _osrm = osrm;
            _notifications = notifications;
            _logger = logger;
        }

        public Task StartDispatchAsync(Guid orderId)
        {
            // Fire-and-forget background loop. Errors are logged.
            _ = Task.Run(() => RunDispatchLoopAsync(orderId));
            return Task.CompletedTask;
        }

        public async Task<bool> AcceptOfferAsync(Guid orderId, Guid driverId)
        {
            using var scope = _sp.CreateScope();
            var orders = scope.ServiceProvider.GetRequiredService<ITaxiOrderRepository>();
            var drivers = scope.ServiceProvider.GetRequiredService<ITaxiDriverRepository>();

            var won = await orders.AssignDriverAsync(orderId, driverId);
            if (!won) return false;

            await drivers.UpdateStatusAsync(driverId, DriverStatus.OnOrder);

            if (_offers.TryGetValue(orderId, out var state) && state.CurrentDriverId == driverId)
                state.Tcs.TrySetResult(OfferOutcome.Accepted);

            var order = await orders.GetByIdAsync(orderId);
            if (order != null)
            {
                await _hub.Clients.Group(Groups.Client(order.ClientId)).SendAsync("OrderAssigned", new
                {
                    orderId = order.OrderId,
                    driverId,
                    assignedAt = order.AssignedAt
                });
            }
            return true;
        }

        public Task DeclineOfferAsync(Guid orderId, Guid driverId)
        {
            if (_offers.TryGetValue(orderId, out var state) && state.CurrentDriverId == driverId)
                state.Tcs.TrySetResult(OfferOutcome.Declined);
            return Task.CompletedTask;
        }

        private async Task RunDispatchLoopAsync(Guid orderId)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var orders = scope.ServiceProvider.GetRequiredService<ITaxiOrderRepository>();
                var drivers = scope.ServiceProvider.GetRequiredService<ITaxiDriverRepository>();

                var order = await orders.GetByIdAsync(orderId);
                if (order == null || order.Status != OrderStatus.Searching)
                {
                    _logger.LogInformation("Dispatch loop skipped for {OrderId}: status {Status}",
                        orderId, order?.Status);
                    return;
                }

                var candidates = await drivers.GetAvailableNearAsync(
                    order.PickupLatitude, order.PickupLongitude, order.VehicleType,
                    SearchRadiusKm, CandidateLimit);

                if (candidates.Count == 0)
                {
                    _logger.LogInformation("Dispatch: no candidates near pickup for order {OrderId}", orderId);
                    await orders.UpdateStatusAsync(orderId, OrderStatus.Cancelled);
                    await NotifyClientNoDrivers(order.ClientId, orderId);
                    return;
                }

                var pickup = new GeoPoint(order.PickupLatitude, order.PickupLongitude);
                var dests = candidates
                    .Select(d => new GeoPoint(d.CurrentLatitude!.Value, d.CurrentLongitude!.Value))
                    .ToList();
                var durations = await _osrm.GetDurationsAsync(pickup, dests);

                IEnumerable<TaxiDriver> sorted = candidates;
                if (durations != null && durations.Length == candidates.Count)
                {
                    sorted = candidates
                        .Select((d, i) => (Driver: d, Eta: durations[i]))
                        .OrderBy(x => x.Eta)
                        .Select(x => x.Driver);
                }

                foreach (var driver in sorted.Take(OfferTopN))
                {
                    // Re-check order is still up for grabs
                    var fresh = await orders.GetByIdAsync(orderId);
                    if (fresh == null || fresh.Status != OrderStatus.Searching)
                        return;

                    var state = new OfferState(driver.DriverId);
                    _offers[orderId] = state;

                    var offerBody = new
                    {
                        orderId,
                        pickupLat = order.PickupLatitude,
                        pickupLng = order.PickupLongitude,
                        destinationLat = order.DestinationLatitude,
                        destinationLng = order.DestinationLongitude,
                        pickupAddress = order.PickupAddress,
                        destinationAddress = order.DestinationAddress,
                        vehicleType = order.VehicleType.ToString(),
                        price = order.OrderPrice,
                        timeoutSeconds = (int)OfferTimeout.TotalSeconds
                    };

                    if (driver.User != null)
                        await _notifications.SendAsync(driver.User, new NotificationPayload("OrderOffer", offerBody));
                    else
                        await _hub.Clients.Group(Groups.Driver(driver.DriverId)).SendAsync("OrderOffer", offerBody);

                    var completed = await Task.WhenAny(state.Tcs.Task, Task.Delay(OfferTimeout));
                    var outcome = completed == state.Tcs.Task ? state.Tcs.Task.Result : OfferOutcome.Timeout;

                    _offers.TryRemove(orderId, out _);

                    if (outcome == OfferOutcome.Accepted)
                        return;
                    // Declined or timeout -> next candidate
                }

                // No one accepted
                var still = await orders.GetByIdAsync(orderId);
                if (still != null && still.Status == OrderStatus.Searching)
                {
                    await orders.UpdateStatusAsync(orderId, OrderStatus.Cancelled);
                    await NotifyClientNoDrivers(order.ClientId, orderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dispatch loop failed for order {OrderId}", orderId);
            }
        }

        private Task NotifyClientNoDrivers(Guid clientId, Guid orderId)
        {
            return _hub.Clients.Group(Groups.Client(clientId))
                .SendAsync("OrderDispatchFailed", new { orderId, reason = "no_drivers_available" });
        }

        private enum OfferOutcome { Accepted, Declined, Timeout }

        private sealed class OfferState
        {
            public Guid CurrentDriverId { get; }
            public TaskCompletionSource<OfferOutcome> Tcs { get; }
            public OfferState(Guid driverId)
            {
                CurrentDriverId = driverId;
                Tcs = new TaskCompletionSource<OfferOutcome>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }
    }

    public static class Groups
    {
        public static string Client(Guid clientId) => $"client:{clientId}";
        public static string Driver(Guid driverId) => $"driver:{driverId}";
        public const string Drivers = "drivers";
    }
}
