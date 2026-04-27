namespace Mobile.Shared.Models;

public enum OrderStatus
{
    Created,
    Searching,
    Assigned,
    DriverEnRoute,
    DriverArrived,
    InProgress,
    Completed,
    Cancelled
}

public enum DriverStatus
{
    Offline,
    Available,
    OnOrder
}

public enum VehicleType
{
    Standart,
    Premium,
    Luxury
}
