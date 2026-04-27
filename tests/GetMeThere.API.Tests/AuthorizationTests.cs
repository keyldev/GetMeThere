using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace GetMeThere.API.Tests;

public class AuthorizationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public AuthorizationTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Taxi_order_without_jwt_returns_401()
    {
        var client = _factory.CreateClient();
        var resp = await client.PostAsJsonAsync("/api/v1/taxi/order", new
        {
            pickupLatitude = 55.7,
            pickupLongitude = 37.6,
            destinationLatitude = 55.8,
            destinationLongitude = 37.7,
            vehicleType = 0,
            orderPrice = 200
        });
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task User_info_without_jwt_returns_401()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/v1/user/info");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
