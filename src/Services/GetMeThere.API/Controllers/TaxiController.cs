using GetMeThere.API.Models;
using GetMeThere.API.Repositories;
using GetMeThere.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GetMeThere.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaxiController : ControllerBase
    {
        private readonly ITaxiOrderService _orderService;
        private readonly IDispatchService _dispatch;
        private readonly ITaxiDriverRepository _drivers;

        public TaxiController(
            ITaxiOrderService orderService,
            IDispatchService dispatch,
            ITaxiDriverRepository drivers)
        {
            _orderService = orderService;
            _dispatch = dispatch;
            _drivers = drivers;
        }

        [HttpPost("order")]
        public async Task<IActionResult> CreateOrder([FromBody] TaxiOrder order)
        {
            if (!TryGetUserId(out var clientId)) return Unauthorized();
            var saved = await _orderService.CreateAsync(order, clientId);
            return Ok(new { orderId = saved.OrderId, status = saved.Status.ToString() });
        }

        [HttpPost("orders/{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            if (!TryGetUserId(out var clientId)) return Unauthorized();
            return Map(await _orderService.CancelAsync(id, clientId));
        }

        [HttpPost("orders/{id:guid}/accept")]
        public async Task<IActionResult> Accept(Guid id)
        {
            var driverId = await ResolveDriverIdAsync();
            if (driverId == null) return Forbid();
            var won = await _dispatch.AcceptOfferAsync(id, driverId.Value);
            return won ? Ok() : Conflict(new { message = "Order already taken or not in offered state" });
        }

        [HttpPost("orders/{id:guid}/decline")]
        public async Task<IActionResult> Decline(Guid id)
        {
            var driverId = await ResolveDriverIdAsync();
            if (driverId == null) return Forbid();
            await _dispatch.DeclineOfferAsync(id, driverId.Value);
            return Ok();
        }

        [HttpPost("orders/{id:guid}/en-route")]
        public async Task<IActionResult> EnRoute(Guid id)
        {
            var driverId = await ResolveDriverIdAsync();
            if (driverId == null) return Forbid();
            return Map(await _orderService.MarkEnRouteAsync(id, driverId.Value));
        }

        [HttpPost("orders/{id:guid}/arrived")]
        public async Task<IActionResult> Arrived(Guid id)
        {
            var driverId = await ResolveDriverIdAsync();
            if (driverId == null) return Forbid();
            return Map(await _orderService.MarkArrivedAsync(id, driverId.Value));
        }

        [HttpPost("orders/{id:guid}/start")]
        public async Task<IActionResult> StartTrip(Guid id)
        {
            var driverId = await ResolveDriverIdAsync();
            if (driverId == null) return Forbid();
            return Map(await _orderService.StartTripAsync(id, driverId.Value));
        }

        [HttpPost("orders/{id:guid}/complete")]
        public async Task<IActionResult> CompleteTrip(Guid id)
        {
            var driverId = await ResolveDriverIdAsync();
            if (driverId == null) return Forbid();
            return Map(await _orderService.CompleteTripAsync(id, driverId.Value));
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(raw, out userId);
        }

        private async Task<Guid?> ResolveDriverIdAsync()
        {
            if (!TryGetUserId(out var userId)) return null;
            var driver = await _drivers.GetByUserIdAsync(userId);
            return driver?.DriverId;
        }

        private IActionResult Map(LifecycleResult result) => result switch
        {
            LifecycleResult.Ok => Ok(),
            LifecycleResult.NotFound => NotFound(),
            LifecycleResult.NotOwned => Forbid(),
            LifecycleResult.InvalidState => Conflict(new { message = "Order is not in a state that allows this transition" }),
            _ => StatusCode(500)
        };
    }
}
