using Mobile.Driver.ViewModels;

namespace Mobile.Driver.Pages;

public partial class InTripPage : ContentPage
{
    public InTripPage(InTripViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
