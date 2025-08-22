using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Mappers
{
    public class OperationsMappingProfile : Profile
    {
        public OperationsMappingProfile()
        {
            // These maps are used when an agent updates the status of a vehicle or an issue.
            // We use ReverseMap() to allow mapping in both directions if needed,
            // but primarily it's for applying the DTO changes to the model.
            CreateMap<UpdateVehicleStatusDTO, Vehicle>().ReverseMap();
            CreateMap<UpdateIssueStatusDTO, Issue>().ReverseMap();
        }
    }
}