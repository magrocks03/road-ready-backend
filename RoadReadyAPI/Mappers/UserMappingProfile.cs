using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoadReadyAPI.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // Maps the input from the registration form to the User model
            CreateMap<RegisterUserDTO, User>();
        }
    }
}