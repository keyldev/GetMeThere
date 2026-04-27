using Mobile.Rider.Pages;

namespace Mobile.Rider;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("destination", typeof(DestinationSearchPage));
        Routing.RegisterRoute("confirm", typeof(ConfirmOrderPage));
    }
}
