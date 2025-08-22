using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class UpdateBookingStatusDTO
    {
        [Required]
        public string StatusName { get; set; } = string.Empty; // e.g., "Completed", "In Progress"
    }
}