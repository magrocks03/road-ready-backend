namespace RoadReadyAPI.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;
        public string FuelType { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public int SeatingCapacity { get; set; }

        // --- NEW PROPERTY ADDED HERE ---
        public double AverageRating { get; set; } = 0;
    }
}