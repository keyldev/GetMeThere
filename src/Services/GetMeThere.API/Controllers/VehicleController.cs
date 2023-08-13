using GetMeThere.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GetMeThere.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VehicleController : ControllerBase
    {
        public VehicleController()
        {
            
        }

        [HttpPut("edit")]
        
        public IActionResult EditVehicle([FromBody] Vehicle vehicle)
        {
            throw new NotImplementedException();
        }
        [HttpPost("add")]
        public IActionResult AddVehicle([FromBody] Vehicle vehicle)
        {
            throw new NotImplementedException();
        }

    }
}
