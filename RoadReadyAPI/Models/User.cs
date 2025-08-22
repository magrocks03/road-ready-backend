using System;
using System.Collections.Generic;

namespace RoadReadyAPI.Models
{
    /// <summary>
    /// Represents the Users table, now with fields for password reset functionality.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordHashKey { get; set; } = new byte[0];
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        // --- NEW PROPERTIES ADDED HERE ---
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiresAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}