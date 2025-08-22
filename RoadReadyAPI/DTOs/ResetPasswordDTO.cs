using RoadReadyAPI.Validators; // <-- Add this using statement
using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        // --- CHANGE IS HERE ---
        [PasswordStrength] // Our new custom validation attribute
        public string NewPassword { get; set; } = string.Empty;
    }
}