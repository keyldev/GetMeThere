namespace GetMeThere.API.Models.DTO
{
    public class DriverProfileDto
    {
        public Guid DriverId { get; set; }
        public string Name { get; set; } = "";
        public string VehicleModel { get; set; } = "";
        public string VehicleColor { get; set; } = "";
        public string LicensePlate { get; set; } = "";
        public string VehicleType { get; set; } = "";
    }
}
