using RoadReadyAPI.Validators;
using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class AdminCreateUserDTO
    {
        [Required(ErrorMessage = "First name cannot be empty")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name cannot be empty")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty")]
        [PasswordStrength]
        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role name cannot be empty")]
        public string RoleName { get; set; } = string.Empty; // e.g., "Admin", "Rental Agent"
    }
}
