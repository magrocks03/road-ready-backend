namespace RoadReadyAPI.Models
{
    public class Refund
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public int? IssueId { get; set; }
        public Issue? Issue { get; set; }
        public decimal Amount { get; set; }
        public DateTime RefundDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int AdminUserId { get; set; }
        public User AdminUser { get; set; } = null!;
    }
}
