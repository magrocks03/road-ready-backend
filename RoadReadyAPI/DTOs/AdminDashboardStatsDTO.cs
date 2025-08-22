using System.Collections.Generic;

namespace RoadReadyAPI.DTOs
{
    public class AdminDashboardStatsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<MostPopularVehicleDTO> MostPopularVehicles { get; set; } = new List<MostPopularVehicleDTO>();
    }
}
