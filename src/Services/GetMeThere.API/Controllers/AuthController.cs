using GetMeThere.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GetMeThere.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        public AuthController(IConfiguration configuration, ITokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }
        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            if (username == "Tom" && password == "tom123")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username)
                };
                var token = _tokenService.GetAccessToken(claims);
                return Ok(token);
            }
            else
            {
                return NotFound(new
                {
                    message = "User not found"
                });
            }
        }

    }
}
