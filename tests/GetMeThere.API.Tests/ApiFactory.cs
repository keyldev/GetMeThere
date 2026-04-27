using GetMeThere.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GetMeThere.API.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public string DbName { get; } = "test-" + Guid.NewGuid();

    static ApiFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Key", "test-key-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "taxi_web_service");
        Environment.SetEnvironmentVariable("Jwt__Audience", "taxi_cross_app");
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection",
            "server=localhost;user=root;password=stub;database=stub");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Scrub everything EF Core / Pomelo touched so the InMemory provider can register cleanly.
            var efDescriptors = services.Where(d =>
                d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore") == true ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                d.ImplementationType?.FullName?.StartsWith("Pomelo") == true ||
                d.ImplementationType?.FullName?.StartsWith("Microsoft.EntityFrameworkCore") == true)
                .ToList();
            foreach (var d in efDescriptors) services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(opts => opts.UseInMemoryDatabase(DbName));

            // Clear all health check registrations (the MySQL one would fail in tests).
            services.PostConfigure<HealthCheckServiceOptions>(opts => opts.Registrations.Clear());
        });
    }
}
