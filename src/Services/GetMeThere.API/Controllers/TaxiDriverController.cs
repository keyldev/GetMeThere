using GetMeThere.API.Hubs;
using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using GetMeThere.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace GetMeThere.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaxiDriverController : ControllerBase
    {
        private readonly IHubContext<TaxiHub> _taxiHubContext;
        private readonly ITaxiDriverService _taxiDriverService;
        

        public TaxiDriverController(ITaxiDriverService taxiDriverService)
        {
            _taxiDriverService = taxiDriverService;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddNewDriver([FromBody] TaxiDriver driver)
        {
            throw new NotImplementedException();
        }
        [HttpGet("get/all")]
        public async Task<IActionResult> GetAllDrivers()
        {
            throw new NotImplementedException();
        }
        [HttpPost("order/accept")]
        public async Task<IActionResult> AcceptOrder([FromBody] OrderAcceptDto order)
        {
            _taxiDriverService.SendOrderAccepted(order);
            
            return Ok();
        }
    }
}
