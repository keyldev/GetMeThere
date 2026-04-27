using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Driver.Pages;
using Mobile.Driver.Services;
using Mobile.Shared.Configuration;
using Mobile.Shared.Models;

namespace Mobile.Driver.ViewModels;

public partial class OnlineViewModel : ObservableObject
{
    private const double MoscowLat = 55.7558;
    private const double MoscowLng = 37.6176;

    private readonly DriverHubClient _hub;
    private readonly LocationStreamer _streamer;
    private readonly IServiceProvider _sp;
    private readonly AppConfig _cfg;

    [ObservableProperty] private bool _isOnline;
    [ObservableProperty] private string _statusText = "Готов выйти на линию";
    [ObservableProperty] private double? _currentLatitude;
    [ObservableProperty] private double? _currentLongitude;

    // Demo earnings — static values for the mock UX
    public int OrdersToday => 7;
    public int RubEarned => 4200;
    public double HoursOnline => 3.5;

    public OnlineViewModel(DriverHubClient hub, LocationStreamer streamer, IServiceProvider sp, AppConfig cfg)
    {
        _hub = hub;
        _streamer = streamer;
        _sp = sp;
        _cfg = cfg;

        _hub.OrderOffer += offer =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var modal = _sp.GetRequiredService<IncomingOrderModal>();
                if (modal.BindingContext is IncomingOrderViewModel vm)
                    vm.LoadOffer(offer);
                await Shell.Current.Navigation.PushModalAsync(modal);
            });
        };
    }

    [RelayCommand]
    private async Task ToggleAsync()
    {
        if (IsOnline)
        {
            try { await _hub.GoOfflineAsync(); } catch { }
            _streamer.Stop();
            IsOnline = false;
            StatusText = "Оффлайн";
        }
        else
        {
            StatusText = "Подключаемся…";
            try
            {
                await _hub.ConnectAsync();
                await _hub.GoOnlineAsync();
                _streamer.Start();
                IsOnline = true;
                StatusText = "На линии — ждём заказ";
            }
            catch (Exception ex)
            {
                StatusText = $"Не удалось выйти на линию: {ex.Message}";
            }
        }
    }

    public async Task RefreshLocationAsync()
    {
        try
        {
            var loc = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5)));
            if (loc != null)
            {
                CurrentLatitude = loc.Latitude;
                CurrentLongitude = loc.Longitude;
                return;
            }
        }
        catch { /* fall through to fallback */ }

        if (_cfg.UseMocks)
        {
            CurrentLatitude = MoscowLat;
            CurrentLongitude = MoscowLng;
        }
    }
}
