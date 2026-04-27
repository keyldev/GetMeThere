using Mobile.Rider.ViewModels;

namespace Mobile.Rider.Pages;

public partial class CompletePage : ContentPage
{
    public CompletePage(CompleteViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
