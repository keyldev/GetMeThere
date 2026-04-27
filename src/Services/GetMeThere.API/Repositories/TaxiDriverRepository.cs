using GetMeThere.API.Data;
using GetMeThere.API.Models;
using GetMeThere.API.Services;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Repositories
{
    public class TaxiDriverRepository : ITaxiDriverRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TaxiDriverRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<TaxiDriver?> GetByIdAsync(Guid driverId)
        {
            return _dbContext.TaxiDrivers
                .Include(d => d.User)
                .Include(d => d.Vehicle)
                .FirstOrDefaultAsync(d => d.DriverId == driverId);
        }

        public Task<TaxiDriver?> GetByUserIdAsync(Guid userId)
        {
            return _dbContext.TaxiDrivers
                .Include(d => d.Vehicle)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<bool> UpdateLocationAsync(Guid driverId, double lat, double lng, DateTime updatedAt)
        {
            var rows = await _dbContext.TaxiDrivers
                .Where(d => d.DriverId == driverId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.CurrentLatitude, lat)
                    .SetProperty(d => d.CurrentLongitude, lng)
                    .SetProperty(d => d.LastLocationUpdate, updatedAt));
            return rows > 0;
        }

        public async Task<bool> UpdateStatusAsync(Guid driverId, DriverStatus status)
        {
            var rows = await _dbContext.TaxiDrivers
                .Where(d => d.DriverId == driverId)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.Status, status));
            return rows > 0;
        }

        public async Task<TaxiDriver> AddDriverWithVehicleAsync(Guid userId, Vehicle vehicle)
        {
            vehicle.VehicleId = Guid.NewGuid();
            _dbContext.Vehicles.Add(vehicle);

            var driver = new TaxiDriver
            {
                DriverId = Guid.NewGuid(),
                UserId = userId,
                VehicleId = vehicle.VehicleId,
                Status = DriverStatus.Offline
            };
            _dbContext.TaxiDrivers.Add(driver);

            await _dbContext.SaveChangesAsync();
            return driver;
        }

        public async Task<List<TaxiDriver>> GetAvailableNearAsync(double lat, double lng, VehicleType vehicleType, double radiusKm, int limit)
        {
            // Pre-filter on bounding box (cheap, indexable in MySQL), then refine in memory with Haversine.
            // 1 deg lat ≈ 111 km; 1 deg lng ≈ 111 * cos(lat) km. Use a generous box.
            var latDelta = radiusKm / 111.0;
            var lngDelta = radiusKm / (111.0 * Math.Max(0.1, Math.Cos(lat * Math.PI / 180.0)));

            var candidates = await _dbContext.TaxiDrivers
                .Include(d => d.Vehicle)
                .Include(d => d.User)
                .Where(d => d.Status == DriverStatus.Available
                            && d.CurrentLatitude != null && d.CurrentLongitude != null
                            && d.Vehicle != null && d.Vehicle.Type == vehicleType
                            && d.CurrentLatitude >= lat - latDelta && d.CurrentLatitude <= lat + latDelta
                            && d.CurrentLongitude >= lng - lngDelta && d.CurrentLongitude <= lng + lngDelta)
                .ToListAsync();

            return candidates
                .Select(d => new
                {
                    Driver = d,
                    Distance = GeoUtils.HaversineKm(lat, lng, d.CurrentLatitude!.Value, d.CurrentLongitude!.Value)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Take(limit)
                .Select(x => x.Driver)
                .ToList();
        }
    }
}
