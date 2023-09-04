using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Services
{
    public interface ITaxiDriverService
    {
        Task SendOrderAccepted(OrderAcceptDto orderAcceptDto);
        Task<List<TaxiDriverDto>> GetAllDriversAsync();
    }
}
