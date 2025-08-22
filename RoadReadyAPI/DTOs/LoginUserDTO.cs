﻿using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class LoginUserDTO
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; } = string.Empty;
    }
}