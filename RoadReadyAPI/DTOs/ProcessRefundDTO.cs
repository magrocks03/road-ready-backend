using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class ProcessRefundDTO
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 10)]
        public string Reason { get; set; } = string.Empty;

        // --- NEW PROPERTY ADDED HERE ---
        // This is optional. If not provided, the full booking cost will be refunded.
        [Range(0.01, double.MaxValue)]
        public decimal? Amount { get; set; }
    }
}