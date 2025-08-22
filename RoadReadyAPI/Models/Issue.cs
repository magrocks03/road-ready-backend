namespace RoadReadyAPI.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public string IssueDescription { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }
        public string Status { get; set; } = string.Empty; // e.g., 'Open', 'Resolved'
        public string? AdminNotes { get; set; }
    }
}
