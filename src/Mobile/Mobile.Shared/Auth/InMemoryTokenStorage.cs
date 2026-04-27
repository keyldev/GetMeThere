namespace Mobile.Shared.Auth;

/// <summary>
/// Process-local token storage. Used in mock/demo mode to avoid platform SecureStorage
/// issues (e.g. Android emulator without screen lock).
/// </summary>
public sealed class InMemoryTokenStorage : ITokenStorage
{
    private string? _access;
    private string? _refresh;

    public Task<string?> GetAccessAsync() => Task.FromResult(_access);
    public Task<string?> GetRefreshAsync() => Task.FromResult(_refresh);

    public Task SetAsync(string access, string refresh)
    {
        _access = access;
        _refresh = refresh;
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _access = null;
        _refresh = null;
        return Task.CompletedTask;
    }
}
