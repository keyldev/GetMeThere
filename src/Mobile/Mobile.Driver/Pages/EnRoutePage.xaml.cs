using Mobile.Driver.ViewModels;

namespace Mobile.Driver.Pages;

public partial class EnRoutePage : ContentPage
{
    public EnRoutePage(EnRouteViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
