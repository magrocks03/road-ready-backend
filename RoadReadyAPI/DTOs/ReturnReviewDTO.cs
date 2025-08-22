using System;

namespace RoadReadyAPI.DTOs
{
    public class ReturnReviewDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        // Details about who left the review
        public int UserId { get; set; }
        public string CustomerFirstName { get; set; } = string.Empty;

        // Details about the vehicle being reviewed
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
    }
}