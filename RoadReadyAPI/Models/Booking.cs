using System;
using System.Collections.Generic;

namespace RoadReadyAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int StatusId { get; set; }
        public BookingStatus Status { get; set; } = null!;
        public ICollection<BookingExtra> BookingExtras { get; set; } = new List<BookingExtra>();

        // --- NEW PROPERTY ADDED HERE ---
        // This establishes the one-to-one relationship with Payment.
        public Payment? Payment { get; set; }
    }
}