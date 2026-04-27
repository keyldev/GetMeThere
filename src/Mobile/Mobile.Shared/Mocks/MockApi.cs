using System.Net;
using Mobile.Shared.Models;
using Mobile.Shared.Net;

namespace Mobile.Shared.Mocks;

public sealed class MockApi : IApi
{
    private readonly MockHubBus _bus;
    private static readonly Guid FakeDriverId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    public MockApi(MockHubBus bus)
    {
        _bus = bus;
    }

    public Task<AuthResultDto> Login(LoginRequest req) =>
        Task.FromResult(new AuthResultDto
        {
            AccessToken = "mock-access-" + Guid.NewGuid().ToString("N")[..8],
            RefreshToken = "mock-refresh-" + Guid.NewGuid().ToString("N")[..8]
        });

    public Task<AuthResultDto> Register(RegisterRequest req) =>
        Task.FromResult(new AuthResultDto
        {
            AccessToken = "mock-access-" + Guid.NewGuid().ToString("N")[..8],
            RefreshToken = "mock-refresh-" + Guid.NewGuid().ToString("N")[..8]
        });

    public Task<AuthResultDto> Refresh(RefreshRequest req) =>
        Task.FromResult(new AuthResultDto
        {
            AccessToken = "mock-access-refreshed",
            RefreshToken = "mock-refresh-refreshed"
        });

    public Task<UserDto> GetMe() =>
        Task.FromResult(new UserDto
        {
            Id = Guid.NewGuid(),
            Login = "demo",
            Email = "demo@example.com",
            Name = "Демо Пользователь"
        });

    public Task<DriverRegisteredDto> RegisterDriver(RegisterDriverDto dto) =>
        Task.FromResult(new DriverRegisteredDto
        {
            DriverId = FakeDriverId,
            VehicleId = Guid.NewGuid()
        });

    public Task<DriverProfileDto> GetDriverProfile(Guid id) =>
        Task.FromResult(new DriverProfileDto
        {
            DriverId = id,
            Name = "Иван Петров",
            VehicleModel = "Toyota Camry",
            VehicleColor = "белый",
            LicensePlate = "А123БВ77",
            VehicleType = "Standart"
        });

    public Task<CreateOrderResponse> CreateOrder(CreateOrderRequest req)
    {
        var orderId = Guid.NewGuid();
        _ = RunRideScenarioAsync(orderId, req);
        return Task.FromResult(new CreateOrderResponse
        {
            OrderId = orderId,
            Status = "Searching"
        });
    }

    public Task CancelOrder(Guid id)
    {
        _bus.RaiseOrderCancelled(new OrderEventDto { OrderId = id });
        return Task.CompletedTask;
    }

    public Task<HttpResponseMessage> AcceptOrder(Guid id) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

    public Task DeclineOrder(Guid id) => Task.CompletedTask;
    public Task EnRoute(Guid id) => Task.CompletedTask;
    public Task Arrived(Guid id) => Task.CompletedTask;
    public Task StartTrip(Guid id) => Task.CompletedTask;
    public Task CompleteTrip(Guid id) => Task.CompletedTask;

    private async Task RunRideScenarioAsync(Guid orderId, CreateOrderRequest req)
    {
        var pickupLat = req.PickupLatitude;
        var pickupLng = req.PickupLongitude;
        var destLat = req.DestinationLatitude;
        var destLng = req.DestinationLongitude;

        // Driver starts ~500m southwest of pickup.
        var startLat = pickupLat - 0.005;
        var startLng = pickupLng - 0.007;

        // Pre-fetch real road routes (driver→pickup, pickup→destination) so movement
        // follows actual streets, not a straight line.
        var preRouteTask = MockRouting.GetRoutePointsAsync(startLat, startLng, pickupLat, pickupLng, 8);
        var rideRouteTask = MockRouting.GetRoutePointsAsync(pickupLat, pickupLng, destLat, destLng, 10);

        await Task.Delay(2_000);
        _bus.RaiseOrderAssigned(new OrderAssignedDto
        {
            OrderId = orderId,
            DriverId = FakeDriverId,
            AssignedAt = DateTime.UtcNow
        });

        var preRoute = await preRouteTask;
        for (var i = 0; i < preRoute.Count; i++)
        {
            await Task.Delay(1_500);
            var pt = preRoute[i];
            _bus.RaiseDriverLocation(new DriverLocationDto
            {
                OrderId = orderId,
                Latitude = pt.Lat,
                Longitude = pt.Lng,
                At = DateTime.UtcNow
            });
            if (i == 0)
                _bus.RaiseDriverEnRoute(new OrderEventDto { OrderId = orderId });
        }

        await Task.Delay(800);
        _bus.RaiseDriverArrived(new OrderEventDto { OrderId = orderId });

        await Task.Delay(2_500);
        _bus.RaiseTripStarted(new OrderEventDto { OrderId = orderId });

        var rideRoute = await rideRouteTask;
        foreach (var pt in rideRoute)
        {
            await Task.Delay(1_500);
            _bus.RaiseDriverLocation(new DriverLocationDto
            {
                OrderId = orderId,
                Latitude = pt.Lat,
                Longitude = pt.Lng,
                At = DateTime.UtcNow
            });
        }

        await Task.Delay(800);
        _bus.RaiseTripCompleted(new OrderEventDto { OrderId = orderId });
    }
}
