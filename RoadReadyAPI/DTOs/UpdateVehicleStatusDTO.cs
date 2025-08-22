using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class UpdateVehicleStatusDTO
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}