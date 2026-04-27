using GetMeThere.API.Models;
using GetMeThere.API.Repositories;
using GetMeThere.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GetMeThere.API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaxiHub : Hub
    {
        private readonly ITaxiHubService _hubService;
        private readonly ITaxiDriverRepository _drivers;
        private readonly ITaxiOrderRepository _orders;
        private readonly ILogger<TaxiHub> _logger;

        public TaxiHub(
            ITaxiHubService hubService,
            ITaxiDriverRepository drivers,
            ITaxiOrderRepository orders,
            ILogger<TaxiHub> logger)
        {
            _hubService = hubService;
            _drivers = drivers;
            _orders = orders;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var username = Context.User?.Identity?.Name;
            if (userId == null || username == null)
            {
                Context.Abort();
                return;
            }

            await _hubService.UpdateUserConnectionId(username, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, Services.Groups.Client(userId.Value));

            var driver = await _drivers.GetByUserIdAsync(userId.Value);
            if (driver != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Services.Groups.Driver(driver.DriverId));
                await Groups.AddToGroupAsync(Context.ConnectionId, Services.Groups.Drivers);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            var username = Context.User?.Identity?.Name;
            if (username != null)
                await _hubService.UpdateUserConnectionId(username, "");

            if (userId != null)
            {
                var driver = await _drivers.GetByUserIdAsync(userId.Value);
                if (driver != null)
                    await _drivers.UpdateStatusAsync(driver.DriverId, DriverStatus.Offline);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Driver clients call this every few seconds with their current GPS position.
        public async Task SendLocation(double latitude, double longitude)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var driver = await _drivers.GetByUserIdAsync(userId.Value);
            if (driver == null) return;

            var now = DateTime.UtcNow;
            await _drivers.UpdateLocationAsync(driver.DriverId, latitude, longitude, now);

            var activeOrder = await _orders.GetActiveByDriverAsync(driver.DriverId);
            if (activeOrder != null)
            {
                await Clients.Group(Services.Groups.Client(activeOrder.ClientId))
                    .SendAsync("DriverLocation", new
                    {
                        orderId = activeOrder.OrderId,
                        latitude,
                        longitude,
                        at = now
                    });
            }
        }

        // Driver explicitly toggles availability.
        public async Task GoOnline()
        {
            var userId = GetUserId();
            if (userId == null) return;
            var driver = await _drivers.GetByUserIdAsync(userId.Value);
            if (driver == null) return;
            await _drivers.UpdateStatusAsync(driver.DriverId, DriverStatus.Available);
        }

        public async Task GoOffline()
        {
            var userId = GetUserId();
            if (userId == null) return;
            var driver = await _drivers.GetByUserIdAsync(userId.Value);
            if (driver == null) return;
            await _drivers.UpdateStatusAsync(driver.DriverId, DriverStatus.Offline);
        }

        private Guid? GetUserId()
        {
            var raw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }
}
