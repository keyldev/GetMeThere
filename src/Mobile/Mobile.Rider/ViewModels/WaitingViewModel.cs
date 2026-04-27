using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Rider.Services;
using Mobile.Shared.Mocks;
using Mobile.Shared.Net;

namespace Mobile.Rider.ViewModels;

public enum OrderStage { Searching, Assigned, EnRoute, Arrived, InProgress }

public partial class WaitingViewModel : ObservableObject, IQueryAttributable
{
    private readonly RiderHubClient _hub;
    private readonly IApi _api;

    [ObservableProperty] private Guid _orderId;
    [ObservableProperty] private double _pickupLat;
    [ObservableProperty] private double _pickupLng;
    [ObservableProperty] private double _destLat;
    [ObservableProperty] private double _destLng;

    [ObservableProperty] private string _statusText = "Ищем водителя…";
    [ObservableProperty] private Guid? _assignedDriverId;
    [ObservableProperty] private string? _driverName;
    [ObservableProperty] private string? _driverVehicleSummary;
    [ObservableProperty] private double? _driverLatitude;
    [ObservableProperty] private double? _driverLongitude;
    [ObservableProperty] private bool _canCancel = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stage1Done), nameof(Stage2Done), nameof(Stage3Done),
                              nameof(Stage4Done), nameof(Stage5Done))]
    private OrderStage _stage = OrderStage.Searching;

    public bool Stage1Done => Stage >= OrderStage.Searching;
    public bool Stage2Done => Stage >= OrderStage.Assigned;
    public bool Stage3Done => Stage >= OrderStage.EnRoute;
    public bool Stage4Done => Stage >= OrderStage.Arrived;
    public bool Stage5Done => Stage >= OrderStage.InProgress;

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

    public WaitingViewModel(RiderHubClient hub, IApi api)
    {
        _hub = hub;
        _api = api;

        _hub.OrderAssigned += e =>
        {
            if (e.OrderId != OrderId) return;
            AssignedDriverId = e.DriverId;
            StatusText = "Водитель найден, едет к вам";
            Stage = OrderStage.Assigned;
            _ = LoadDriverProfileAsync(e.DriverId);
        };
        _hub.DriverLocation += e =>
        {
            if (e.OrderId != OrderId) return;
            DriverLatitude = e.Latitude;
            DriverLongitude = e.Longitude;
        };
        _hub.DriverEnRoute += e =>
        {
            if (e.OrderId != OrderId) return;
            StatusText = "Водитель в пути";
            Stage = OrderStage.EnRoute;
        };
        _hub.DriverArrived += async e =>
        {
            if (e.OrderId != OrderId) return;
            StatusText = "Водитель прибыл";
            Stage = OrderStage.Arrived;
            try { Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(400)); } catch { }
            await Task.CompletedTask;
        };
        _hub.TripStarted += e =>
        {
            if (e.OrderId != OrderId) return;
            Stage = OrderStage.InProgress;
            MainThread.BeginInvokeOnMainThread(async () =>
                await Shell.Current.GoToAsync("//trip", new Dictionary<string, object>
                {
                    ["orderId"] = OrderId,
                    ["pickupLat"] = PickupLat,
                    ["pickupLng"] = PickupLng,
                    ["destLat"] = DestLat,
                    ["destLng"] = DestLng
                }));
        };
        _hub.OrderCancelled += e =>
        {
            if (e.OrderId != OrderId) return;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                StatusText = "Заказ отменён";
                await Task.Delay(1500);
                await Shell.Current.GoToAsync("//map");
            });
        };
        _hub.OrderDispatchFailed += e =>
        {
            if (e.OrderId != OrderId) return;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                StatusText = "Свободных водителей рядом нет";
                await Task.Delay(2000);
                await Shell.Current.GoToAsync("//map");
            });
        };
    }

    public Task ConnectHubAsync() => _hub.ConnectAsync();

    public async Task LoadRouteAsync()
    {
        if (PickupLat == 0 || DestLat == 0) return;
        try
        {
            RoutePoints = await MockRouting.GetRoutePointsAsync(PickupLat, PickupLng, DestLat, DestLng, 30);
            RouteReady?.Invoke();
        }
        catch { /* keep null on failure */ }
    }

    private async Task LoadDriverProfileAsync(Guid driverId)
    {
        try
        {
            var profile = await _api.GetDriverProfile(driverId);
            DriverName = profile.Name;
            var color = string.IsNullOrWhiteSpace(profile.VehicleColor) ? "" : profile.VehicleColor + " ";
            DriverVehicleSummary = $"{color}{profile.VehicleModel} · {profile.LicensePlate}".Trim();
        }
        catch { /* leave fields null on failure */ }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        CanCancel = false;
        try { await _api.CancelOrder(OrderId); }
        catch { /* ignore — UI will receive OrderCancelled event */ }
        await Shell.Current.GoToAsync("//map");
    }
}
