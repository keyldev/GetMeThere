namespace Mobile.Shared.Models;

public class CreateOrderRequest
{
    public string? PickupAddress { get; set; }
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }

    public string? DestinationAddress { get; set; }
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }

    public VehicleType VehicleType { get; set; } = VehicleType.Standart;
    public float OrderPrice { get; set; }
    public bool NeedChildSeat { get; set; }
    public int SeatsCount { get; set; } = 2;
    public DateTime PickupTime { get; set; } = DateTime.UtcNow;
}

public class CreateOrderResponse
{
    public Guid OrderId { get; set; }
    public string? Status { get; set; }
}

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

public class DriverProfileDto
{
    public Guid DriverId { get; set; }
    public string Name { get; set; } = "";
    public string VehicleModel { get; set; } = "";
    public string VehicleColor { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string VehicleType { get; set; } = "";
}
