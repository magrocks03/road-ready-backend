using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Mappers
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            // This map is simple because the property names match.
            CreateMap<CreateReviewDTO, Review>();

            // This map is more complex and requires custom rules.
            CreateMap<Review, ReturnReviewDTO>()
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.Vehicle.Id))
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle.Name));
        }
    }
}
