using CommunityToolkit.Mvvm.ComponentModel;
using Mobile.Rider.Services;
using Mobile.Shared.Mocks;

namespace Mobile.Rider.ViewModels;

public partial class TripViewModel : ObservableObject, IQueryAttributable
{
    private readonly RiderHubClient _hub;

    [ObservableProperty] private Guid _orderId;
    [ObservableProperty] private double _pickupLat;
    [ObservableProperty] private double _pickupLng;
    [ObservableProperty] private double _destLat;
    [ObservableProperty] private double _destLng;

    [ObservableProperty] private string _statusText = "Поездка началась";
    [ObservableProperty] private double? _driverLatitude;
    [ObservableProperty] private double? _driverLongitude;

    public IReadOnlyList<(double Lat, double Lng)>? RoutePoints { get; private set; }
    public event Action? RouteReady;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("orderId", out var raw))
        {
            if (raw is Guid g) OrderId = g;
            else if (Guid.TryParse(raw?.ToString(), out var id)) OrderId = id;
        }
        if (query.TryGetValue("pickupLat", out var v) && DestinationSearchViewModel.TryGetDouble(v, out var d)) PickupLat = d;
        if (query.TryGetValue("pickupLng", out v) && DestinationSearchViewModel.TryGetDouble(v, out d)) PickupLng = d;
        if (query.TryGetValue("destLat", out v) && DestinationSearchViewModel.TryGetDouble(v, out d)) DestLat = d;
        if (query.TryGetValue("destLng", out v) && DestinationSearchViewModel.TryGetDouble(v, out d)) DestLng = d;
    }

    public TripViewModel(RiderHubClient hub)
    {
        _hub = hub;
        _hub.DriverLocation += e =>
        {
            if (e.OrderId != OrderId) return;
            DriverLatitude = e.Latitude;
            DriverLongitude = e.Longitude;
        };
        _hub.TripCompleted += e =>
        {
            if (e.OrderId != OrderId) return;
            MainThread.BeginInvokeOnMainThread(async () =>
                await Shell.Current.GoToAsync("//complete", new Dictionary<string, object> { ["orderId"] = OrderId }));
        };
    }

    public async Task LoadRouteAsync()
    {
        if (PickupLat == 0 || DestLat == 0) return;
        try
        {
            RoutePoints = await MockRouting.GetRoutePointsAsync(PickupLat, PickupLng, DestLat, DestLng, 30);
            RouteReady?.Invoke();
        }
        catch { }
    }
}
