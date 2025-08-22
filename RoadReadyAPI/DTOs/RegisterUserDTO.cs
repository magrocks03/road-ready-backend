using RoadReadyAPI.Validators; // <-- Add this using statement
using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "First name cannot be empty")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name cannot be empty")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty")]
        // --- CHANGE IS HERE ---
        [PasswordStrength] // Our new custom validation attribute
        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}