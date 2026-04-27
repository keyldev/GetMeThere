using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Shared.Models;
using Mobile.Shared.Net;
using Refit;

namespace Mobile.Driver.ViewModels;

public partial class RegisterDriverViewModel : ObservableObject
{
    private readonly IApi _api;

    [ObservableProperty] private string _licensePlate = "";
    [ObservableProperty] private string _model = "";
    [ObservableProperty] private string _color = "";
    [ObservableProperty] private int _seatsCount = 4;
    [ObservableProperty] private bool _haveChildSeat;
    [ObservableProperty] private VehicleType _selectedVehicleType = VehicleType.Standart;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public IReadOnlyList<VehicleType> VehicleTypes { get; } =
        new[] { VehicleType.Standart, VehicleType.Premium, VehicleType.Luxury };

    public RegisterDriverViewModel(IApi api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            await _api.RegisterDriver(new RegisterDriverDto
            {
                LicensePlate = LicensePlate,
                Model = Model,
                Color = Color,
                SeatsCount = SeatsCount,
                HaveChildSeat = HaveChildSeat,
                VehicleType = SelectedVehicleType
            });
            await Shell.Current.GoToAsync("//online");
        }
        catch (ApiException ex) when ((int)ex.StatusCode == 409)
        {
            // already registered — just proceed
            await Shell.Current.GoToAsync("//online");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Не удалось зарегистрировать авто: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task SkipAsync() => Shell.Current.GoToAsync("//online");
}
