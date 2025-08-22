using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Mappers
{
    public class ProfileMappingProfile : Profile
    {
        public ProfileMappingProfile()
        {
            // Maps the User model to the DTO for returning profile information
            CreateMap<User, ReturnUserProfileDTO>();

            // Maps the DTO for updating a profile to the User model
            CreateMap<UpdateUserProfileDTO, User>();
        }
    }
}
