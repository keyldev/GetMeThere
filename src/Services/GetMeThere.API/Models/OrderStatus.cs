namespace GetMeThere.API.Models
{
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
}
