using GetMeThere.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GetMeThere.API.Models;

namespace GetMeThere.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TaxiController : ControllerBase
    {
        private readonly IHubContext<TaxiHub> _taxiHubContext;
        public TaxiController(IHubContext<TaxiHub> taxiHubContext)
        {
            _taxiHubContext = taxiHubContext;
        }
        [HttpPost("order")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // taxiorder
        public async Task<IActionResult> PostOrder([FromBody] TaxiOrder order)
        {

            await _taxiHubContext.Clients.All.SendAsync("NewOrder", order);
            return Ok();
        }
    }
}
