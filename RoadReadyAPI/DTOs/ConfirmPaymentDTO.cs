using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class ConfirmPaymentDTO
    {
        [Required]
        public string MockCardNumber { get; set; } = string.Empty; // For simulation

        [Required]
        public string MockExpiryDate { get; set; } = string.Empty; // For simulation

        [Required]
        public string MockCvc { get; set; } = string.Empty; // For simulation
    }
}
