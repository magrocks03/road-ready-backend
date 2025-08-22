using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Mappers
{
    public class HelperMappingProfile : Profile
    {
        public HelperMappingProfile()
        {
            // Maps the Brand model to the ReturnBrandDTO
            CreateMap<Brand, ReturnBrandDTO>();

            // Maps the Location model to the ReturnLocationDTO
            CreateMap<Location, ReturnLocationDTO>();

            // Maps the Extra model to the ReturnExtraDTO
            CreateMap<Extra, ReturnExtraDTO>()
                .ForMember(dest => dest.ExtraId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
