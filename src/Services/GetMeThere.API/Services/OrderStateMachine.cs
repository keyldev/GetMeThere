using GetMeThere.API.Models;

namespace GetMeThere.API.Services
{
    public static class OrderStateMachine
    {
        private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> Allowed = new()
        {
            [OrderStatus.Created]       = new() { OrderStatus.Searching, OrderStatus.Cancelled },
            [OrderStatus.Searching]     = new() { OrderStatus.Assigned, OrderStatus.Cancelled },
            [OrderStatus.Assigned]      = new() { OrderStatus.DriverEnRoute, OrderStatus.Cancelled, OrderStatus.Searching },
            [OrderStatus.DriverEnRoute] = new() { OrderStatus.DriverArrived, OrderStatus.Cancelled },
            [OrderStatus.DriverArrived] = new() { OrderStatus.InProgress, OrderStatus.Cancelled },
            [OrderStatus.InProgress]    = new() { OrderStatus.Completed },
            [OrderStatus.Completed]     = new(),
            [OrderStatus.Cancelled]     = new()
        };

        public static bool CanTransition(OrderStatus from, OrderStatus to)
        {
            return Allowed.TryGetValue(from, out var next) && next.Contains(to);
        }
    }
}
