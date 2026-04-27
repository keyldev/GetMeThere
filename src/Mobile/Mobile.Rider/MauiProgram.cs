using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Mobile.Rider.Pages;
using Mobile.Rider.Services;
using Mobile.Rider.ViewModels;
using Mobile.Shared;
using Mobile.Shared.Configuration;

namespace Mobile.Rider;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMobileShared(new AppConfig { UseMocks = true });

        builder.Services.AddSingleton<NominatimClient>();
        builder.Services.AddSingleton<OsrmRouteClient>();
        builder.Services.AddSingleton<RiderHubClient>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<MapViewModel>();
        builder.Services.AddTransient<DestinationSearchViewModel>();
        builder.Services.AddTransient<ConfirmOrderViewModel>();
        builder.Services.AddTransient<WaitingViewModel>();
        builder.Services.AddTransient<TripViewModel>();
        builder.Services.AddTransient<CompleteViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<DestinationSearchPage>();
        builder.Services.AddTransient<ConfirmOrderPage>();
        builder.Services.AddTransient<WaitingPage>();
        builder.Services.AddTransient<TripPage>();
        builder.Services.AddTransient<CompletePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
