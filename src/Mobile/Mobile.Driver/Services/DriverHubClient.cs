using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Shared.Configuration;
using Mobile.Shared.Hub;
using Mobile.Shared.Mocks;
using Mobile.Shared.Models;
using Mobile.Shared.Net;

namespace Mobile.Driver.Services;

public sealed class DriverHubClient : IAsyncDisposable
{
    private readonly HubConnectionFactory _factory;
    private readonly AppConfig _cfg;
    private readonly MockHubBus _bus;
    private HubConnection? _hub;
    private bool _mockSubscribed;
    private CancellationTokenSource? _mockOfferCts;

    public event Action<OrderOfferDto>? OrderOffer;

    public DriverHubClient(HubConnectionFactory factory, AppConfig cfg, MockHubBus bus)
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
            _bus.OrderOffer += e => OrderOffer?.Invoke(e);
            _mockSubscribed = true;
            return;
        }

        if (_hub != null) return;
        _hub = _factory.Create();
        _hub.On<OrderOfferDto>(TaxiHubEvents.OrderOffer, e => OrderOffer?.Invoke(e));
        await _hub.StartAsync(ct);
    }

    public Task SendLocationAsync(double lat, double lng)
    {
        if (_cfg.UseMocks) return Task.CompletedTask;
        return _hub == null ? Task.CompletedTask : _hub.InvokeAsync(HubMethods.SendLocation, lat, lng);
    }

    public Task GoOnlineAsync()
    {
        if (_cfg.UseMocks)
        {
            ScheduleMockOffer();
            return Task.CompletedTask;
        }
        return _hub == null ? Task.CompletedTask : _hub.InvokeAsync(HubMethods.GoOnline);
    }

    public Task GoOfflineAsync()
    {
        if (_cfg.UseMocks)
        {
            _mockOfferCts?.Cancel();
            _mockOfferCts = null;
            return Task.CompletedTask;
        }
        return _hub == null ? Task.CompletedTask : _hub.InvokeAsync(HubMethods.GoOffline);
    }

    private void ScheduleMockOffer()
    {
        _mockOfferCts?.Cancel();
        _mockOfferCts = new CancellationTokenSource();
        var ct = _mockOfferCts.Token;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
                _bus.RaiseOrderOffer(new OrderOfferDto
                {
                    OrderId = Guid.NewGuid(),
                    PickupLat = 55.7616,
                    PickupLng = 37.6080,
                    PickupAddress = "Тверская улица, 10, Москва",
                    DestinationLat = 55.7539,
                    DestinationLng = 37.6208,
                    DestinationAddress = "Красная площадь, Москва",
                    VehicleType = "Standart",
                    Price = 350f,
                    TimeoutSeconds = 15
                });
            }
            catch (OperationCanceledException) { /* offline cancelled */ }
        }, ct);
    }

    public async ValueTask DisposeAsync()
    {
        _mockOfferCts?.Cancel();
        if (_hub != null)
        {
            await _hub.DisposeAsync();
            _hub = null;
        }
    }
}
