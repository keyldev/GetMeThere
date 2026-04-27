using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Shared.Auth;
using Mobile.Shared.Models;
using Mobile.Shared.Net;

namespace Mobile.Rider.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IApi _api;
    private readonly ITokenStorage _tokens;

    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy;

    public LoginViewModel(IApi api, ITokenStorage tokens)
    {
        _api = api;
        _tokens = tokens;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            var auth = await _api.Login(new LoginRequest { Username = Username, Password = Password });
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
            ErrorMessage = $"Не удалось войти: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task GoToRegisterAsync() => Shell.Current.GoToAsync("//register");
}
