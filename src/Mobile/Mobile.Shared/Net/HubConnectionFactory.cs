using Microsoft.AspNetCore.SignalR.Client;
using Mobile.Shared.Auth;
using Mobile.Shared.Configuration;

namespace Mobile.Shared.Net;

public sealed class HubConnectionFactory
{
    private readonly ITokenStorage _tokens;
    private readonly AppConfig _cfg;

    public HubConnectionFactory(ITokenStorage tokens, AppConfig cfg)
    {
        _tokens = tokens;
        _cfg = cfg;
    }

    public HubConnection Create() =>
        new HubConnectionBuilder()
            .WithUrl(_cfg.HubUrl, opts =>
            {
                opts.AccessTokenProvider = async () => await _tokens.GetAccessAsync();
            })
            .WithAutomaticReconnect()
            .Build();
}
