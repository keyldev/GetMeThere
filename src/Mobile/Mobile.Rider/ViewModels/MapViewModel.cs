using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Shared.Configuration;

namespace Mobile.Rider.ViewModels;

public partial class MapViewModel : ObservableObject
{
    private const double MoscowLat = 55.7558;
    private const double MoscowLng = 37.6176;

    private readonly AppConfig _cfg;

    [ObservableProperty] private double? _currentLatitude;
    [ObservableProperty] private double? _currentLongitude;
    [ObservableProperty] private string _statusText = "Получаем вашу позицию…";

    public MapViewModel(AppConfig cfg)
    {
        _cfg = cfg;
    }

    public async Task LoadCurrentLocationAsync()
    {
        try
        {
            var loc = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5)));
            if (loc != null)
            {
                CurrentLatitude = loc.Latitude;
                CurrentLongitude = loc.Longitude;
                StatusText = "Куда едем?";
                return;
            }
        }
        catch
        {
            // fall through to fallback below
        }

        if (_cfg.UseMocks)
        {
            CurrentLatitude = MoscowLat;
            CurrentLongitude = MoscowLng;
            StatusText = "Куда едем? (демо)";
        }
        else
        {
            StatusText = "Не удалось определить позицию";
        }
    }

    [RelayCommand]
    private async Task ChooseDestinationAsync()
    {
        if (CurrentLatitude == null || CurrentLongitude == null)
        {
            StatusText = "Сначала нужно получить вашу позицию";
            return;
        }
        await Shell.Current.GoToAsync($"destination?fromLat={CurrentLatitude}&fromLng={CurrentLongitude}");
    }
}
