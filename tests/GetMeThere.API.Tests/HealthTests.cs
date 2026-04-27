using FluentAssertions;
using Xunit;

namespace GetMeThere.API.Tests;

public class HealthTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public HealthTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Health_endpoint_responds_ok()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/health");
        resp.IsSuccessStatusCode.Should().BeTrue();
    }
}
