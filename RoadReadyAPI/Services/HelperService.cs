using AutoMapper;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Linq; // <-- Add this using statement
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class HelperService : IHelperService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IExtraRepository _extraRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HelperService> _logger;

        public HelperService(
            IBrandRepository brandRepository,
            ILocationRepository locationRepository,
            IExtraRepository extraRepository,
            IMapper mapper,
            ILogger<HelperService> logger)
        {
            _brandRepository = brandRepository;
            _locationRepository = locationRepository;
            _extraRepository = extraRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ReturnBrandDTO>> GetAllBrandsAsync()
        {
            _logger.LogInformation("Fetching all brands.");
            // --- FIX IS HERE ---
            // We use the synchronous ToList() method, which works perfectly in our tests.
            var brands = _brandRepository.GetAll().ToList();
            return _mapper.Map<List<ReturnBrandDTO>>(brands);
        }

        public async Task<List<ReturnLocationDTO>> GetAllLocationsAsync()
        {
            _logger.LogInformation("Fetching all locations.");
            // --- FIX IS HERE ---
            var locations = _locationRepository.GetAll().ToList();
            return _mapper.Map<List<ReturnLocationDTO>>(locations);
        }

        public async Task<List<ReturnExtraDTO>> GetAllExtrasAsync()
        {
            _logger.LogInformation("Fetching all extras.");
            // --- FIX IS HERE ---
            var extras = _extraRepository.GetAll().ToList();
            return _mapper.Map<List<ReturnExtraDTO>>(extras);
        }
    }
}