using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class InitiateBookingDTO
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public List<int>? ExtraIds { get; set; } = new List<int>();
    }
}
