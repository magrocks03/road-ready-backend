using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class UpdateUserRoleDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}