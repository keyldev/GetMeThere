using GetMeThere.API.Hubs;
using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GetMeThere.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaxiDriverController : ControllerBase
    {
        private readonly IHubContext<TaxiHub> _taxiHubContext;

        public TaxiDriverController(IHubContext<TaxiHub> taxiHubContext)
        {
            _taxiHubContext = taxiHubContext;
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
            throw new NotImplementedException();
        }
    }
}
