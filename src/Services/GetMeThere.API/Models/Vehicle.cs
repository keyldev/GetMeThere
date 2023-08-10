using System.ComponentModel.DataAnnotations;

namespace GetMeThere.API.Models
{
    public class Vehicle
    {
        [Key]
        public Guid VehicleId { get; set; }

        public string? Number { get; set; }
        public string? Model { get; set; }
        public int SeatsCount { get; set; }
        public string? Color { get; set; }
        public bool HaveChildSeat { get; set; }
        // by default vehicle type is standart
        public VehicleType Type { get; set; } = VehicleType.Standart;
    }
    public enum VehicleType
    {
        Standart,
        Premium,
        Luxury
    }
}
