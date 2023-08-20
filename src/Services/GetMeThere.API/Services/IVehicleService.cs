using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public interface IVehicleService
    {

        Task<Vehicle> GetVehicleAsync(Guid driverId);
        Task EditVehicleAsync(Guid driverId, Vehicle vehicle);

    }
}
