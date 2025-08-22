using AutoMapper;

namespace RoadReadyAPI.Mappers
{
    public class AdminDashboardMappingProfile : Profile
    {
        public AdminDashboardMappingProfile()
        {
            // No specific mappings are needed for the dashboard stats DTOs
            // as they will be manually constructed in the service layer.
        }
    }
}
