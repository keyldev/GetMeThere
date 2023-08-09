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
        //change it into async task.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequest login)
        {
            var loginResult = await _authService.Login(login);

            if (loginResult == null)
            {
                return NotFound(new
                {
                    message = "Account not found!"
                });
            }
            else return Ok(loginResult);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var registerResult = await _authService.Register(registerRequest);
            if (registerResult == null)
            {
                // add more info about errors
                return BadRequest(new
                {
                    message = "An error has occured"
                });
            }
            else return Ok(registerResult);
        }

    }
}
