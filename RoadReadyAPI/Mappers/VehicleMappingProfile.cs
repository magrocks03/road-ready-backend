using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Mappers
{
    public class VehicleMappingProfile : Profile
    {
        public VehicleMappingProfile()
        {
            // AutoMapper will automatically map the new properties because they have the same name.
            CreateMap<CreateVehicleDTO, Vehicle>();

            CreateMap<Vehicle, ReturnVehicleDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.StoreName));
        }
    }
}