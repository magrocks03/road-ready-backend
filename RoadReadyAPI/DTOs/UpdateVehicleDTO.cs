using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class UpdateVehicleDTO
    {
        [Range(0.01, double.MaxValue)]
        public decimal? PricePerDay { get; set; }
        public bool? IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
    }
}