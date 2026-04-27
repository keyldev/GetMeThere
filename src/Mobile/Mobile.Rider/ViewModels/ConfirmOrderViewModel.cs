using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Rider.Services;
using Mobile.Shared.Models;
using Mobile.Shared.Net;

namespace Mobile.Rider.ViewModels;

public partial class ConfirmOrderViewModel : ObservableObject, IQueryAttributable
{
    private readonly OsrmRouteClient _osrm;
    private readonly IApi _api;

    [ObservableProperty] private double _fromLat;
    [ObservableProperty] private double _fromLng;
    [ObservableProperty] private double _toLat;
    [ObservableProperty] private double _toLng;
    [ObservableProperty] private string? _toAddress;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("fromLat", out var v) && DestinationSearchViewModel.TryGetDouble(v, out var d)) FromLat = d;
        if (query.TryGetValue("fromLng", out v) && DestinationSearchViewModel.TryGetDouble(v, out d)) FromLng = d;
        if (query.TryGetValue("toLat", out v) && DestinationSearchViewModel.TryGetDouble(v, out d)) ToLat = d;
        if (query.TryGetValue("toLng", out v) && DestinationSearchViewModel.TryGetDouble(v, out d)) ToLng = d;
        if (query.TryGetValue("toAddress", out var a)) ToAddress = a is string s ? s : a?.ToString();
    }


    [ObservableProperty] private double _distanceKm;
    [ObservableProperty] private double _durationMin;
    [ObservableProperty] private float _price;
    [ObservableProperty] private VehicleType _selectedVehicleType = VehicleType.Standart;
    [ObservableProperty] private bool _needChildSeat;
    [ObservableProperty] private string? _statusText;
    [ObservableProperty] private bool _isBusy;

    public IReadOnlyList<VehicleType> VehicleTypes { get; } =
        new[] { VehicleType.Standart, VehicleType.Premium, VehicleType.Luxury };

    public ConfirmOrderViewModel(OsrmRouteClient osrm, IApi api)
    {
        _osrm = osrm;
        _api = api;
    }

    public async Task LoadRouteAsync()
    {
        StatusText = "Считаем маршрут…";
        var route = await _osrm.GetRouteAsync(FromLat, FromLng, ToLat, ToLng);
        if (route == null) { StatusText = "Не удалось построить маршрут"; return; }
        DistanceKm = route.DistanceMeters / 1000.0;
        DurationMin = route.DurationSeconds / 60.0;
        RecalculatePrice();
        StatusText = null;
    }

    partial void OnSelectedVehicleTypeChanged(VehicleType value) => RecalculatePrice();

    private void RecalculatePrice()
    {
        var multiplier = SelectedVehicleType switch
        {
            VehicleType.Premium => 1.7f,
            VehicleType.Luxury => 2.5f,
            _ => 1.0f
        };
        Price = (float)Math.Round(80 + DistanceKm * 35 * multiplier);
    }

    [RelayCommand]
    private async Task PlaceOrderAsync()
    {
        IsBusy = true;
        StatusText = "Отправляем заказ…";
        try
        {
            var resp = await _api.CreateOrder(new CreateOrderRequest
            {
                PickupLatitude = FromLat,
                PickupLongitude = FromLng,
                DestinationLatitude = ToLat,
                DestinationLongitude = ToLng,
                DestinationAddress = ToAddress,
                VehicleType = SelectedVehicleType,
                NeedChildSeat = NeedChildSeat,
                OrderPrice = Price,
                SeatsCount = 2,
                PickupTime = DateTime.UtcNow
            });
            await Shell.Current.GoToAsync("//waiting", new Dictionary<string, object>
            {
                ["orderId"] = resp.OrderId,
                ["pickupLat"] = FromLat,
                ["pickupLng"] = FromLng,
                ["destLat"] = ToLat,
                ["destLng"] = ToLng
            });
        }
        catch (Exception ex)
        {
            StatusText = $"Заказ не отправлен: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
