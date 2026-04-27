using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Shared.Net;

namespace Mobile.Driver.ViewModels;

public partial class EnRouteViewModel : ObservableObject, IQueryAttributable
{
    private readonly IApi _api;

    [ObservableProperty] private Guid _orderId;
    [ObservableProperty] private double _pickupLat;
    [ObservableProperty] private double _pickupLng;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("orderId", out var raw))
        {
            if (raw is Guid g) OrderId = g;
            else if (Guid.TryParse(raw?.ToString(), out var id)) OrderId = id;
        }
        if (query.TryGetValue("pickupLat", out var v) && TryGetDouble(v, out var d)) PickupLat = d;
        if (query.TryGetValue("pickupLng", out v) && TryGetDouble(v, out d)) PickupLng = d;
    }

    private static bool TryGetDouble(object? v, out double d)
    {
        if (v is double dv) { d = dv; return true; }
        return double.TryParse(v?.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d);
    }

    [ObservableProperty] private string _statusText = "Принят. Едете к пассажиру";
    [ObservableProperty] private bool _hasArrived;
    [ObservableProperty] private string? _errorMessage;

    public EnRouteViewModel(IApi api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task MarkEnRouteAsync()
    {
        try { await _api.EnRoute(OrderId); StatusText = "В пути к пассажиру"; }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    [RelayCommand]
    private async Task MarkArrivedAsync()
    {
        try { await _api.Arrived(OrderId); StatusText = "Прибыл — ждёте пассажира"; HasArrived = true; }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }

    [RelayCommand]
    private async Task StartTripAsync()
    {
        try
        {
            await _api.StartTrip(OrderId);
            await Shell.Current.GoToAsync("//in-trip", new Dictionary<string, object> { ["orderId"] = OrderId });
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }
}
