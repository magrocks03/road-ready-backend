namespace RoadReadyAPI.Models
{
    public class BookingExtra
    {
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public int ExtraId { get; set; }
        public Extra Extra { get; set; } = null!;
    }
}