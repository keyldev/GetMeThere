using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Mobile.Shared.Auth;
using Mobile.Shared.Configuration;
using Mobile.Shared.Models;

namespace Mobile.Shared.Net;

public sealed class AuthenticatedHttpHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokens;
    private readonly AppConfig _cfg;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthenticatedHttpHandler(ITokenStorage tokens, AppConfig cfg)
    {
        _tokens = tokens;
        _cfg = cfg;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        await AttachAccessTokenAsync(request);
        var response = await base.SendAsync(request, ct);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        if (request.Headers.Contains("X-Auth-Retried"))
            return response;

        var refreshed = await TryRefreshAsync(ct);
        if (!refreshed) return response;

        var clone = await CloneRequestAsync(request);
        clone.Headers.Add("X-Auth-Retried", "1");
        await AttachAccessTokenAsync(clone);
        response.Dispose();
        return await base.SendAsync(clone, ct);
    }

    private async Task AttachAccessTokenAsync(HttpRequestMessage request)
    {
        var token = await _tokens.GetAccessAsync();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<bool> TryRefreshAsync(CancellationToken ct)
    {
        await _refreshLock.WaitAsync(ct);
        try
        {
            var refresh = await _tokens.GetRefreshAsync();
            if (string.IsNullOrEmpty(refresh)) return false;

            using var bareClient = new HttpClient { BaseAddress = new Uri(_cfg.BaseUrl) };
            var resp = await bareClient.PostAsJsonAsync("/api/v1/auth/refresh",
                new RefreshRequest { RefreshToken = refresh }, ct);
            if (!resp.IsSuccessStatusCode) return false;

            var auth = await resp.Content.ReadFromJsonAsync<AuthResultDto>(cancellationToken: ct);
            if (auth?.AccessToken == null || auth.RefreshToken == null) return false;

            await _tokens.SetAsync(auth.AccessToken, auth.RefreshToken);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri) { Version = original.Version };
        foreach (var h in original.Headers)
            clone.Headers.TryAddWithoutValidation(h.Key, h.Value);

        if (original.Content != null)
        {
            var bytes = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var h in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
        }
        return clone;
    }
}
