using Microsoft.Maui.Storage;

namespace Mobile.Shared.Auth;

public sealed class SecureStorageTokenStorage : ITokenStorage
{
    private const string AccessKey = "gmt_access_token";
    private const string RefreshKey = "gmt_refresh_token";

    public Task<string?> GetAccessAsync() => SecureStorage.Default.GetAsync(AccessKey);
    public Task<string?> GetRefreshAsync() => SecureStorage.Default.GetAsync(RefreshKey);

    public async Task SetAsync(string access, string refresh)
    {
        await SecureStorage.Default.SetAsync(AccessKey, access);
        await SecureStorage.Default.SetAsync(RefreshKey, refresh);
    }

    public Task ClearAsync()
    {
        SecureStorage.Default.Remove(AccessKey);
        SecureStorage.Default.Remove(RefreshKey);
        return Task.CompletedTask;
    }
}
