using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Mappers
{
    public class IssueMappingProfile : Profile
    {
        public IssueMappingProfile()
        {
            CreateMap<CreateIssueDTO, Issue>();

            CreateMap<Issue, ReturnIssueDTO>()
                .ForMember(dest => dest.IssueId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Booking.UserId))
                // --- FIX IS HERE ---
                // This logic now correctly handles cases where the last name might be null or empty.
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Booking.User.LastName)
                    ? src.Booking.User.FirstName
                    : $"{src.Booking.User.FirstName} {src.Booking.User.LastName}"))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.Booking.VehicleId))
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Booking.Vehicle.Name));
        }
    }
}
