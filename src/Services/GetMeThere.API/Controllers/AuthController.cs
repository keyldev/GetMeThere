using GetMeThere.API.Models;
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
        private readonly IAuthService _authService;
        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }
        [HttpPost("login")]
        public IActionResult Login(LoginRequest login)
        {
            var loginResult = _authService.Login(login);

            if (loginResult == null)
            {
                return NotFound(new
                {
                    message = "Account not found!"
                });
            }
            else return Ok(loginResult);
        }

    }
}
