
using GetMeThere.API.Configuration;
using GetMeThere.API.Data;
using GetMeThere.API.Hubs;
using GetMeThere.API.Notifications;
using GetMeThere.API.Repositories;
using GetMeThere.API.Services;
using GetMeThere.API.Services.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;

namespace GetMeThere.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, _, cfg) => cfg
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File("logs/api-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:o} [{Level:u3}] [{CorrelationId}] {Message:lj} {Properties:j}{NewLine}{Exception}"));

            // Add services to the container.
            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MauiDev", p => p
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
            jwt.Validate();
            builder.Services.AddSingleton(jwt);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Allow SignalR clients to pass JWT via ?access_token= query string on the hub URL.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var token = ctx.Request.Query["access_token"];
                        var path = ctx.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/taxiHub"))
                            ctx.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });
            // Di for Services
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<ITaxiOrderRepository, TaxiOrderRepository>();
            builder.Services.AddScoped<ITaxiOrderService, TaxiOrderService>();

            builder.Services.AddScoped<ITaxiDriverRepository, TaxiDriverRepository>();
            builder.Services.AddScoped<ITaxiDriverService, TaxiDriverService>();

            builder.Services.AddScoped<ITaxiHubService, TaxiHubService>();
            builder.Services.AddScoped<ITaxiHubRepository, TaxiHubRepository>();

            builder.Services.AddSingleton<INotificationChannel, SignalRChannel>();
            builder.Services.AddSingleton<INotificationChannel, PushChannel>();
            builder.Services.AddSingleton<INotificationDispatcher, NotificationDispatcher>();

            builder.Services.AddSingleton<IDispatchService, DispatchService>();
            builder.Services.AddHttpClient<IOsrmClient, OsrmClient>((sp, http) =>
            {
                var baseUrl = sp.GetRequiredService<IConfiguration>().GetSection("Osrm")["BaseUrl"]
                    ?? "https://router.project-osrm.org/";
                if (!baseUrl.EndsWith('/')) baseUrl += "/";
                http.BaseAddress = new Uri(baseUrl);
                http.Timeout = TimeSpan.FromSeconds(5);
            });
            


            // DI for database
            // dependency injection for services
            var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(dbConnectionString, new MySqlServerVersion(new Version(8, 0, 32))));

            builder.Services.AddHealthChecks()
                .AddMySql(dbConnectionString!, name: "mysql", tags: new[] { "db", "ready" });

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("MauiDev");
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<TaxiHub>("/taxiHub");
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}