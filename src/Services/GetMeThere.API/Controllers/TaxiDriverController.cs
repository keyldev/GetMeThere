using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
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
    public class TaxiDriverController : ControllerBase
    {
        private readonly ITaxiDriverService _taxiDriverService;

        public TaxiDriverController(ITaxiDriverService taxiDriverService)
        {
            _taxiDriverService = taxiDriverService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDriverDto dto)
        {
            var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(raw, out var userId)) return Unauthorized();

            var result = await _taxiDriverService.RegisterDriverAsync(userId, dto);
            if (result == null)
                return Conflict(new { message = "Driver already registered for this user" });
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var profile = await _taxiDriverService.GetProfileAsync(id);
            return profile == null ? NotFound() : Ok(profile);
        }
    }
}
