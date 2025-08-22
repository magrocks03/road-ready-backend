namespace RoadReadyAPI.DTOs
{
    public class MostPopularVehicleDTO
    {
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
    }
}