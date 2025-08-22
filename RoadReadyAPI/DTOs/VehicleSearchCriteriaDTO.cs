namespace RoadReadyAPI.DTOs
{
    public class VehicleSearchCriteriaDTO : PaginationDTO // <-- INHERITS FROM PAGINATION DTO
    {
        public int? LocationId { get; set; }
        public int? BrandId { get; set; }
        public string? Model { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsAvailable { get; set; } = true;
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public int? SeatingCapacity { get; set; }
        // --- NEW PROPERTY ADDED HERE ---
        public string? BrandName { get; set; } // For partial text search on brand

        public string? Name { get; set; } // For partial tect search on name
    }
}
