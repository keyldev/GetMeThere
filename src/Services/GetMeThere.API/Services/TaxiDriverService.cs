using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using GetMeThere.API.Repositories;

namespace GetMeThere.API.Services
{
    public class TaxiDriverService : ITaxiDriverService
    {
        private readonly ITaxiDriverRepository _drivers;

        public TaxiDriverService(ITaxiDriverRepository drivers)
        {
            _drivers = drivers;
        }

        public async Task<DriverRegisteredDto?> RegisterDriverAsync(Guid userId, RegisterDriverDto dto)
        {
            var existing = await _drivers.GetByUserIdAsync(userId);
            if (existing != null) return null;

            var vehicle = new Vehicle
            {
                Number = dto.LicensePlate,
                Model = dto.Model,
                Color = dto.Color,
                SeatsCount = dto.SeatsCount,
                HaveChildSeat = dto.HaveChildSeat,
                Type = dto.VehicleType
            };

            var driver = await _drivers.AddDriverWithVehicleAsync(userId, vehicle);
            return new DriverRegisteredDto { DriverId = driver.DriverId, VehicleId = driver.VehicleId };
        }

        public async Task<DriverProfileDto?> GetProfileAsync(Guid driverId)
        {
            var driver = await _drivers.GetByIdAsync(driverId);
            if (driver == null) return null;
            return new DriverProfileDto
            {
                DriverId = driver.DriverId,
                Name = driver.User?.Name ?? "",
                VehicleModel = driver.Vehicle?.Model ?? "",
                VehicleColor = driver.Vehicle?.Color ?? "",
                LicensePlate = driver.Vehicle?.Number ?? "",
                VehicleType = (driver.Vehicle?.Type ?? Models.VehicleType.Standart).ToString()
            };
        }
    }
}
