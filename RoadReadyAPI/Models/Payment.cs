namespace RoadReadyAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
