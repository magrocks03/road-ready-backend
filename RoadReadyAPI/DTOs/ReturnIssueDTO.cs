using System;

namespace RoadReadyAPI.DTOs
{
    public class ReturnIssueDTO
    {
        public int IssueId { get; set; }
        public int BookingId { get; set; }
        public string IssueDescription { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AdminNotes { get; set; }

        // Details about the user and vehicle involved
        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
    }
}