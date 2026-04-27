using Mobile.Shared.Models;

namespace Mobile.Shared.Mocks;

/// <summary>
/// In-process pub-sub used in mock mode to replace the SignalR hub.
/// MockApi pushes events here; RiderHubClient/DriverHubClient subscribe.
/// </summary>
public sealed class MockHubBus
{
    public event Action<OrderAssignedDto>? OrderAssigned;
    public event Action<DriverLocationDto>? DriverLocation;
    public event Action<OrderEventDto>? DriverEnRoute;
    public event Action<OrderEventDto>? DriverArrived;
    public event Action<OrderEventDto>? TripStarted;
    public event Action<OrderEventDto>? TripCompleted;
    public event Action<OrderEventDto>? OrderCancelled;
    public event Action<OrderDispatchFailedDto>? OrderDispatchFailed;
    public event Action<OrderOfferDto>? OrderOffer;

    public void RaiseOrderAssigned(OrderAssignedDto e) => OrderAssigned?.Invoke(e);
    public void RaiseDriverLocation(DriverLocationDto e) => DriverLocation?.Invoke(e);
    public void RaiseDriverEnRoute(OrderEventDto e) => DriverEnRoute?.Invoke(e);
    public void RaiseDriverArrived(OrderEventDto e) => DriverArrived?.Invoke(e);
    public void RaiseTripStarted(OrderEventDto e) => TripStarted?.Invoke(e);
    public void RaiseTripCompleted(OrderEventDto e) => TripCompleted?.Invoke(e);
    public void RaiseOrderCancelled(OrderEventDto e) => OrderCancelled?.Invoke(e);
    public void RaiseOrderDispatchFailed(OrderDispatchFailedDto e) => OrderDispatchFailed?.Invoke(e);
    public void RaiseOrderOffer(OrderOfferDto e) => OrderOffer?.Invoke(e);
}
