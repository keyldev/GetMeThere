namespace Mobile.Shared.Hub;

public static class TaxiHubEvents
{
    public const string OrderOffer = "OrderOffer";
    public const string OrderAssigned = "OrderAssigned";
    public const string OrderCancelled = "OrderCancelled";
    public const string DriverEnRoute = "DriverEnRoute";
    public const string DriverArrived = "DriverArrived";
    public const string TripStarted = "TripStarted";
    public const string TripCompleted = "TripCompleted";
    public const string DriverLocation = "DriverLocation";
    public const string OrderDispatchFailed = "OrderDispatchFailed";
}

public static class HubMethods
{
    public const string SendLocation = "SendLocation";
    public const string GoOnline = "GoOnline";
    public const string GoOffline = "GoOffline";
}
