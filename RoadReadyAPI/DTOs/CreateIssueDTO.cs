using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class CreateIssueDTO
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Issue description must be between 10 and 500 characters.")]
        public string IssueDescription { get; set; } = string.Empty;
    }
}