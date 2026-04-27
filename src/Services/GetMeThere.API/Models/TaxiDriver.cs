using System.ComponentModel.DataAnnotations;

namespace GetMeThere.API.Models
{
    public class TaxiDriver
    {
        [Key]
        public Guid DriverId { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public DateTime? LastLocationUpdate { get; set; }
        public DriverStatus Status { get; set; } = DriverStatus.Offline;
    }
}
