namespace Mobile.Shared.Auth;

public interface ITokenStorage
{
    Task<string?> GetAccessAsync();
    Task<string?> GetRefreshAsync();
    Task SetAsync(string access, string refresh);
    Task ClearAsync();
}
