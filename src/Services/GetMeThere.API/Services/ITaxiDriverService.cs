using GetMeThere.API.Models.DTO;

namespace GetMeThere.API.Services
{
    public interface ITaxiDriverService
    {
        Task<DriverRegisteredDto?> RegisterDriverAsync(Guid userId, RegisterDriverDto dto);
        Task<DriverProfileDto?> GetProfileAsync(Guid driverId);
    }
}
