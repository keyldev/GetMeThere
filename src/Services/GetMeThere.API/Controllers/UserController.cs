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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var username = (HttpContext.User.Identity as ClaimsIdentity)?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var userInfo = await _userService.GetUserInfo(username);
            if (userInfo == null) return NotFound();
            return Ok(userInfo);
        }
    }
}
