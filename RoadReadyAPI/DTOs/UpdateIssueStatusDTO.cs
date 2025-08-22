using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class UpdateIssueStatusDTO
    {
        [Required]
        public string Status { get; set; } = string.Empty; // e.g., "Resolved", "Under Investigation"

        public string? AdminNotes { get; set; }
    }
}