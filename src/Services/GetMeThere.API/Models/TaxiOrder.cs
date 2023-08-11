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
        //address coords
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }

        public string? DestinationAddress { get; set; } // string + coords
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }

        public VehicleType VehicleType { get; set; } // 0 standart, 1 prem, 2 lux

        public DateTime PickupTime { get; set; }
        public float OrderPrice { get; set; } // km * defaultTaxPerKm + priceByCarType = 7 * 25 + 100 = 270
        //public string? ClientPhoneNumber { get; set; } // yes or not
        public bool IsCompleted { get; set; }
        public bool IsConfirmed { get; set; }
        public bool NeedChildSeat { get; set; } // 
        public int SeatsCount { get; set; }

    }
}
