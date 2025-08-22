namespace RoadReadyAPI.DTOs
{
    public class ReturnInitiateBookingDTO
    {
        public int BookingId { get; set; }
        public decimal TotalCost { get; set; }
        public string Message { get; set; } = "Booking initiated. Please confirm payment to finalize.";
    }
}