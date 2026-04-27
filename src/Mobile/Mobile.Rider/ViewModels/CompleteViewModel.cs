using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Mobile.Rider.ViewModels;

public partial class CompleteViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private Guid _orderId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("orderId", out var raw) && Guid.TryParse(raw?.ToString(), out var id))
            OrderId = id;
    }

    [RelayCommand]
    private Task GoHomeAsync() => Shell.Current.GoToAsync("//map");
}
