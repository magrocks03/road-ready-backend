﻿namespace RoadReadyAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
