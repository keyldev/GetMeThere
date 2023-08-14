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
        private readonly ITaxiOrderService _taxiOrderService;
        public TaxiController(ITaxiOrderService taxiOrderService)
        {
            
            _taxiOrderService = taxiOrderService;


        }
        [HttpPost("order")]
        // taxiorder
        public async Task<IActionResult> PostOrder([FromBody] TaxiOrder order)
        {
            _taxiOrderService.SendNewOrderNotification(order);
            
            return Ok();
        }

        
    }
}
