using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Shared.Net;

namespace Mobile.Driver.ViewModels;

public partial class InTripViewModel : ObservableObject, IQueryAttributable
{
    private readonly IApi _api;

    [ObservableProperty] private Guid _orderId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("orderId", out var raw))
        {
            if (raw is Guid g) OrderId = g;
            else if (Guid.TryParse(raw?.ToString(), out var id)) OrderId = id;
        }
    }

    [ObservableProperty] private string _statusText = "Поездка началась";
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public InTripViewModel(IApi api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task CompleteAsync()
    {
        IsBusy = true;
        try
        {
            await _api.CompleteTrip(OrderId);
            await Shell.Current.GoToAsync("//online");
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsBusy = false; }
    }
}
