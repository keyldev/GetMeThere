using GetMeThere.API.Models;

namespace GetMeThere.API.Repositories
{
    public interface ITaxiDriverRepository
    {
        Task<TaxiDriver?> GetByIdAsync(Guid driverId);
        Task<TaxiDriver?> GetByUserIdAsync(Guid userId);
        Task<bool> UpdateLocationAsync(Guid driverId, double lat, double lng, DateTime updatedAt);
        Task<bool> UpdateStatusAsync(Guid driverId, DriverStatus status);
        Task<List<TaxiDriver>> GetAvailableNearAsync(double lat, double lng, VehicleType vehicleType, double radiusKm, int limit);
        Task<TaxiDriver> AddDriverWithVehicleAsync(Guid userId, Vehicle vehicle);
    }
}
