﻿using System.ComponentModel.DataAnnotations;

namespace GetMeThere.API.Models.DTO
{
    public class OrderAcceptDto
    {
        [Required]
        public Guid DriverId { get; set; }
        [Required]
        public Guid OrderId { get; set; }
    }
}
