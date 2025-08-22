using System.ComponentModel.DataAnnotations;

namespace RoadReadyAPI.DTOs
{
    public class CreateVehicleDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerDay { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string? ImageUrl { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int LocationId { get; set; }

        // --- NEW PROPERTIES ADDED HERE ---
        [Required]
        public string FuelType { get; set; } = string.Empty;

        [Required]
        public string Transmission { get; set; } = string.Empty;

        [Required]
        [Range(2, 15)]
        public int SeatingCapacity { get; set; }
    }
}