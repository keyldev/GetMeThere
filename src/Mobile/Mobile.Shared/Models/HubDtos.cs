namespace Mobile.Shared.Models;

public class OrderOfferDto
{
    public Guid OrderId { get; set; }
    public double PickupLat { get; set; }
    public double PickupLng { get; set; }
    public double DestinationLat { get; set; }
    public double DestinationLng { get; set; }
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
    public string VehicleType { get; set; } = "";
    public float Price { get; set; }
    public int TimeoutSeconds { get; set; }
}

public class OrderAssignedDto
{
    public Guid OrderId { get; set; }
    public Guid DriverId { get; set; }
    public DateTime? AssignedAt { get; set; }
}

public class DriverLocationDto
{
    public Guid OrderId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime At { get; set; }
}

public class OrderEventDto
{
    public Guid OrderId { get; set; }
}

public class OrderDispatchFailedDto
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = "";
}
