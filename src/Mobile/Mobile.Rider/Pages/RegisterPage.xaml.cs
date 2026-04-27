using Mobile.Rider.ViewModels;

namespace Mobile.Rider.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
