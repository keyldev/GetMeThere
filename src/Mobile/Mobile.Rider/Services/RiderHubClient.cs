using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Shared.Configuration;
using Mobile.Shared.Hub;
using Mobile.Shared.Mocks;
using Mobile.Shared.Models;
using Mobile.Shared.Net;

namespace Mobile.Rider.Services;

public sealed class RiderHubClient : IAsyncDisposable
{
    private readonly HubConnectionFactory _factory;
    private readonly AppConfig _cfg;
    private readonly MockHubBus _bus;
    private HubConnection? _hub;
    private bool _mockSubscribed;

    public event Action<OrderAssignedDto>? OrderAssigned;
    public event Action<DriverLocationDto>? DriverLocation;
    public event Action<OrderEventDto>? DriverEnRoute;
    public event Action<OrderEventDto>? DriverArrived;
    public event Action<OrderEventDto>? TripStarted;
    public event Action<OrderEventDto>? TripCompleted;
    public event Action<OrderEventDto>? OrderCancelled;
    public event Action<OrderDispatchFailedDto>? OrderDispatchFailed;

    public RiderHubClient(HubConnectionFactory factory, AppConfig cfg, MockHubBus bus)
    {
        _factory = factory;
        _cfg = cfg;
        _bus = bus;
    }

    public bool IsConnected => _cfg.UseMocks ? _mockSubscribed : _hub?.State == HubConnectionState.Connected;

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        if (_cfg.UseMocks)
        {
            if (_mockSubscribed) return;
            _bus.OrderAssigned += e => OrderAssigned?.Invoke(e);
            _bus.DriverLocation += e => DriverLocation?.Invoke(e);
            _bus.DriverEnRoute += e => DriverEnRoute?.Invoke(e);
            _bus.DriverArrived += e => DriverArrived?.Invoke(e);
            _bus.TripStarted += e => TripStarted?.Invoke(e);
            _bus.TripCompleted += e => TripCompleted?.Invoke(e);
            _bus.OrderCancelled += e => OrderCancelled?.Invoke(e);
            _bus.OrderDispatchFailed += e => OrderDispatchFailed?.Invoke(e);
            _mockSubscribed = true;
            return;
        }

        if (_hub != null) return;
        _hub = _factory.Create();

        _hub.On<OrderAssignedDto>(TaxiHubEvents.OrderAssigned, e => OrderAssigned?.Invoke(e));
        _hub.On<DriverLocationDto>(TaxiHubEvents.DriverLocation, e => DriverLocation?.Invoke(e));
        _hub.On<OrderEventDto>(TaxiHubEvents.DriverEnRoute, e => DriverEnRoute?.Invoke(e));
        _hub.On<OrderEventDto>(TaxiHubEvents.DriverArrived, e => DriverArrived?.Invoke(e));
        _hub.On<OrderEventDto>(TaxiHubEvents.TripStarted, e => TripStarted?.Invoke(e));
        _hub.On<OrderEventDto>(TaxiHubEvents.TripCompleted, e => TripCompleted?.Invoke(e));
        _hub.On<OrderEventDto>(TaxiHubEvents.OrderCancelled, e => OrderCancelled?.Invoke(e));
        _hub.On<OrderDispatchFailedDto>(TaxiHubEvents.OrderDispatchFailed, e => OrderDispatchFailed?.Invoke(e));

        await _hub.StartAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub != null)
        {
            await _hub.DisposeAsync();
            _hub = null;
        }
    }
}
