using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Mobile.Driver.Pages;
using Mobile.Driver.Services;
using Mobile.Driver.ViewModels;
using Mobile.Shared;
using Mobile.Shared.Configuration;

namespace Mobile.Driver;

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

        builder.Services.AddSingleton<DriverHubClient>();
        builder.Services.AddSingleton<LocationStreamer>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterDriverViewModel>();
        builder.Services.AddSingleton<OnlineViewModel>();
        builder.Services.AddTransient<IncomingOrderViewModel>();
        builder.Services.AddTransient<EnRouteViewModel>();
        builder.Services.AddTransient<InTripViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterDriverPage>();
        builder.Services.AddTransient<OnlinePage>();
        builder.Services.AddTransient<IncomingOrderModal>();
        builder.Services.AddTransient<EnRoutePage>();
        builder.Services.AddTransient<InTripPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
