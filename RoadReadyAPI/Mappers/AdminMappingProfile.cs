using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;
using System.Linq;

namespace RoadReadyAPI.Mappers
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            // This map defines how to convert a User object to an AdminReturnUserDTO
            CreateMap<User, AdminReturnUserDTO>()
                // This custom rule tells AutoMapper how to get the list of role names
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));
            // --- NEW MAPPING ADDED HERE ---
            // This map is for an admin creating a new user
            CreateMap<AdminCreateUserDTO, User>();
        }
    }
}