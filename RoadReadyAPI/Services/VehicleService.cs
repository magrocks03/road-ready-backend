using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IVehicleRepository vehicleRepository, IBrandRepository brandRepository, IMapper mapper, ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResultDTO<ReturnVehicleDTO>> SearchVehiclesAsync(VehicleSearchCriteriaDTO criteria)
        {
            _logger.LogInformation("Searching for vehicles based on criteria.");

            // The service now makes two simple, direct calls to the repository.
            var totalCount = await _vehicleRepository.GetTotalVehicleCountAsync(criteria);
            var vehicles = await _vehicleRepository.GetPagedVehiclesAsync(criteria);

            var vehicleDtos = _mapper.Map<List<ReturnVehicleDTO>>(vehicles);

            return new PagedResultDTO<ReturnVehicleDTO>
            {
                Items = vehicleDtos,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };
        }

        // --- Other methods remain the same ---
        public async Task<ReturnVehicleDTO> AddVehicleAsync(CreateVehicleDTO createVehicleDTO)
        {
            var brand = await _brandRepository.GetById(createVehicleDTO.BrandId);
            if (brand == null)
            {
                throw new NoSuchEntityException($"Brand with ID {createVehicleDTO.BrandId} not found.");
            }
            var vehicle = _mapper.Map<Vehicle>(createVehicleDTO);
            var addedVehicle = await _vehicleRepository.Add(vehicle);
            var vehicleWithDetails = await _vehicleRepository.GetVehicleDetailsByIdAsync(addedVehicle.Id);
            return _mapper.Map<ReturnVehicleDTO>(vehicleWithDetails);
        }

        public async Task<bool> DeleteVehicleAsync(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetById(vehicleId);
            if (vehicle == null)
            {
                throw new NoSuchEntityException($"Vehicle with ID {vehicleId} not found.");
            }
            await _vehicleRepository.Delete(vehicleId);
            return true;
        }

        public async Task<ReturnVehicleDTO?> GetVehicleByIdAsync(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(vehicleId);
            if (vehicle == null)
            {
                throw new NoSuchEntityException($"Vehicle with ID {vehicleId} not found.");
            }
            return _mapper.Map<ReturnVehicleDTO>(vehicle);
        }

        public async Task<ReturnVehicleDTO?> UpdateVehicleDetailsAsync(int vehicleId, UpdateVehicleDTO updateVehicleDTO)
        {
            var vehicle = await _vehicleRepository.GetById(vehicleId);
            if (vehicle == null)
            {
                throw new NoSuchEntityException($"Vehicle with ID {vehicleId} not found.");
            }
            if (updateVehicleDTO.PricePerDay.HasValue) vehicle.PricePerDay = updateVehicleDTO.PricePerDay.Value;
            if (updateVehicleDTO.IsAvailable.HasValue) vehicle.IsAvailable = updateVehicleDTO.IsAvailable.Value;
            if (updateVehicleDTO.ImageUrl != null) vehicle.ImageUrl = updateVehicleDTO.ImageUrl;
            await _vehicleRepository.Update(vehicle);
            var updatedVehicleWithDetails = await _vehicleRepository.GetVehicleDetailsByIdAsync(vehicleId);
            return _mapper.Map<ReturnVehicleDTO>(updatedVehicleWithDetails);
        }
    }
}
