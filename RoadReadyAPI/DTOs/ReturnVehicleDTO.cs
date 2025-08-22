namespace RoadReadyAPI.DTOs
{
    public class ReturnVehicleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public int SeatingCapacity { get; set; }

        // --- NEW PROPERTY ADDED HERE ---
        public double AverageRating { get; set; }
    }
}