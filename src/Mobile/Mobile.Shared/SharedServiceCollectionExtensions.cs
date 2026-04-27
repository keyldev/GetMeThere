using Microsoft.Extensions.DependencyInjection;
using Mobile.Shared.Auth;
using Mobile.Shared.Configuration;
using Mobile.Shared.Mocks;
using Mobile.Shared.Net;
using Refit;

namespace Mobile.Shared;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddMobileShared(this IServiceCollection services, AppConfig cfg)
    {
        services.AddSingleton(cfg);
        services.AddSingleton<MockHubBus>();
        services.AddSingleton<HubConnectionFactory>();

        if (cfg.UseMocks)
        {
            services.AddSingleton<ITokenStorage, InMemoryTokenStorage>();
            services.AddSingleton<IApi, MockApi>();
        }
        else
        {
            services.AddSingleton<ITokenStorage, SecureStorageTokenStorage>();
            services.AddTransient<AuthenticatedHttpHandler>();
            services.AddRefitClient<IApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(cfg.BaseUrl))
                .AddHttpMessageHandler<AuthenticatedHttpHandler>();
        }

        return services;
    }
}
