using System.ComponentModel.DataAnnotations;

namespace GetMeThere.API.Models
{
    public class TaxiOrder
    {
        [Key]
        public Guid OrderId { get; set; }
        public Guid ClientId { get; set; }
        public string? ClientName { get; set; }

        public string? PickupAddress { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }

        public string? DestinationAddress { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }

        public VehicleType VehicleType { get; set; } = VehicleType.Standart;

        public DateTime PickupTime { get; set; } = DateTime.UtcNow;
        public float OrderPrice { get; set; }
        public bool NeedChildSeat { get; set; } = false;
        public int SeatsCount { get; set; } = 2;

        public OrderStatus Status { get; set; } = OrderStatus.Created;
        public DateTime? AssignedAt { get; set; }
        public DateTime? DriverArrivedAt { get; set; }
        public DateTime? EstimatedArrival { get; set; }
        public string? RoutePolyline { get; set; }

        public Guid? DriverId { get; set; }
        public TaxiDriver? TaxiDriver { get; set; }
    }
}
