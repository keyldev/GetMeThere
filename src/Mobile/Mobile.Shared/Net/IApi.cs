using Mobile.Shared.Models;
using Refit;

namespace Mobile.Shared.Net;

public interface IApi
{
    [Post("/api/v1/auth/login")]
    Task<AuthResultDto> Login([Body] LoginRequest req);

    [Post("/api/v1/auth/register")]
    Task<AuthResultDto> Register([Body] RegisterRequest req);

    [Post("/api/v1/auth/refresh")]
    Task<AuthResultDto> Refresh([Body] RefreshRequest req);

    [Get("/api/v1/user/info")]
    Task<UserDto> GetMe();

    [Post("/api/v1/taxidriver/register")]
    Task<DriverRegisteredDto> RegisterDriver([Body] RegisterDriverDto dto);

    [Get("/api/v1/taxidriver/{id}")]
    Task<DriverProfileDto> GetDriverProfile(Guid id);

    [Post("/api/v1/taxi/order")]
    Task<CreateOrderResponse> CreateOrder([Body] CreateOrderRequest req);

    [Post("/api/v1/taxi/orders/{id}/cancel")]
    Task CancelOrder(Guid id);

    [Post("/api/v1/taxi/orders/{id}/accept")]
    Task<HttpResponseMessage> AcceptOrder(Guid id);

    [Post("/api/v1/taxi/orders/{id}/decline")]
    Task DeclineOrder(Guid id);

    [Post("/api/v1/taxi/orders/{id}/en-route")]
    Task EnRoute(Guid id);

    [Post("/api/v1/taxi/orders/{id}/arrived")]
    Task Arrived(Guid id);

    [Post("/api/v1/taxi/orders/{id}/start")]
    Task StartTrip(Guid id);

    [Post("/api/v1/taxi/orders/{id}/complete")]
    Task CompleteTrip(Guid id);
}
