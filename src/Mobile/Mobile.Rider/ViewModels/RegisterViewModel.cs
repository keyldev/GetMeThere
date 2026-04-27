using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Shared.Auth;
using Mobile.Shared.Models;
using Mobile.Shared.Net;

namespace Mobile.Rider.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IApi _api;
    private readonly ITokenStorage _tokens;

    [ObservableProperty] private string _login = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public RegisterViewModel(IApi api, ITokenStorage tokens)
    {
        _api = api;
        _tokens = tokens;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            var auth = await _api.Register(new RegisterRequest
            {
                Login = Login,
                Password = Password,
                Email = Email,
                Name = Name
            });
            if (auth?.AccessToken == null || auth.RefreshToken == null)
            {
                ErrorMessage = "Сервер вернул пустой ответ";
                return;
            }
            await _tokens.SetAsync(auth.AccessToken, auth.RefreshToken);
            await Shell.Current.GoToAsync("//map");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Регистрация не удалась: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task GoToLoginAsync() => Shell.Current.GoToAsync("//login");
}
