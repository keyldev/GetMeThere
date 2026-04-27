using Mobile.Rider.ViewModels;

namespace Mobile.Rider.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
