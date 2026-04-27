using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using Xunit;

namespace GetMeThere.API.Tests;

public class AuthTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public AuthTests(ApiFactory factory) => _factory = factory;

    private sealed record AuthResponse(string AccessToken, string RefreshToken);

    [Fact]
    public async Task Register_then_login_returns_tokens()
    {
        var client = _factory.CreateClient();
        var login = "alice-" + Guid.NewGuid().ToString("N")[..8];

        var registerResp = await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Login = login,
            Password = "p@ssw0rd!",
            Email = login + "@example.test",
            Name = "Alice"
        });
        registerResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var registered = await registerResp.Content.ReadFromJsonAsync<AuthResponse>();
        registered.Should().NotBeNull();
        registered!.AccessToken.Should().NotBeNullOrEmpty();
        registered.RefreshToken.Should().NotBeNullOrEmpty();

        var loginResp = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Username = login,
            Password = "p@ssw0rd!"
        });
        loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var loggedIn = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
        loggedIn!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        var client = _factory.CreateClient();
        var login = "bob-" + Guid.NewGuid().ToString("N")[..8];

        await client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Login = login,
            Password = "p@ssw0rd!",
            Email = login + "@example.test",
            Name = "Bob"
        });

        var resp = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Username = login,
            Password = "wrong"
        });
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
