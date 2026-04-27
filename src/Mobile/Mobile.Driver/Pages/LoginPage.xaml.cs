using Mobile.Driver.ViewModels;

namespace Mobile.Driver.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
