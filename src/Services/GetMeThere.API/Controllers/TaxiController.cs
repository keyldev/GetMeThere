using GetMeThere.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GetMeThere.API.Models;
using GetMeThere.API.Services;

namespace GetMeThere.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // bad naming
    public class TaxiController : ControllerBase
    {
        private readonly IHubContext<TaxiHub> _taxiHubContext;
        private readonly ITaxiOrderService _taxOrderService;
        public TaxiController(IHubContext<TaxiHub> taxiHubContext, ITaxiOrderService taxiOrderService)
        {
            _taxiHubContext = taxiHubContext;
            _taxOrderService = taxiOrderService;
        }
        [HttpPost("order")]
        // taxiorder
        public async Task<IActionResult> PostOrder([FromBody] TaxiOrder order)
        {

            await _taxiHubContext.Clients.All.SendAsync("NewOrder", order);
            return Ok();
        }

        
    }
}
