using Mobile.Rider.ViewModels;

namespace Mobile.Rider.Pages;

public partial class DestinationSearchPage : ContentPage
{
    public DestinationSearchPage(DestinationSearchViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
