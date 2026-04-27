using Mobile.Driver.ViewModels;

namespace Mobile.Driver.Pages;

public partial class RegisterDriverPage : ContentPage
{
    public RegisterDriverPage(RegisterDriverViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
