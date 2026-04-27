using Mobile.Rider.ViewModels;

namespace Mobile.Rider.Pages;

public partial class ConfirmOrderPage : ContentPage
{
    private readonly ConfirmOrderViewModel _vm;

    public ConfirmOrderPage(ConfirmOrderViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadRouteAsync();
    }
}
