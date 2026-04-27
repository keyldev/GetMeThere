using Mobile.Driver.ViewModels;

namespace Mobile.Driver.Pages;

public partial class IncomingOrderModal : ContentPage
{
    public IncomingOrderModal(IncomingOrderViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
