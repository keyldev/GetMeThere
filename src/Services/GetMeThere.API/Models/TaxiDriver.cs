using System.ComponentModel.DataAnnotations;

namespace GetMeThere.API.Models
{
    public class TaxiDriver
    {
        [Key]
        public Guid DriverId { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; } // navigation property
        public Guid VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; } // navigation property

        //public ICollection<TaxiOrder> Orders { get; set; }
    }
}
