using GetMeThere.API.Models;

namespace GetMeThere.API.Models.DTO
{
    public class RegisterDriverDto
    {
        public string LicensePlate { get; set; } = "";
        public string Model { get; set; } = "";
        public string Color { get; set; } = "";
        public int SeatsCount { get; set; } = 4;
        public bool HaveChildSeat { get; set; }
        public VehicleType VehicleType { get; set; } = VehicleType.Standart;
    }

    public class DriverRegisteredDto
    {
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
    }
}
