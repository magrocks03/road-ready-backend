using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.Models
{
    public enum PriceType
    {
        PerDay,
        FlatFee
    }

    public class Extra
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public PriceType PriceType { get; set; }

        // Navigation property: An extra can be part of many bookings.
        public ICollection<BookingExtra> BookingExtras { get; set; } = new List<BookingExtra>();
    }
}