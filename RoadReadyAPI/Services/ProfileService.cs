using AutoMapper;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(IUserRepository userRepository, IMapper mapper, ILogger<ProfileService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnUserProfileDTO> GetUserProfileAsync(int userId)
        {
            _logger.LogInformation($"Fetching profile for user ID: {userId}");
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found.");
                throw new NoSuchEntityException("User profile not found.");
            }

            return _mapper.Map<ReturnUserProfileDTO>(user);
        }

        public async Task<ReturnUserProfileDTO> UpdateUserProfileAsync(int userId, UpdateUserProfileDTO updateProfileDTO)
        {
            _logger.LogInformation($"Updating profile for user ID: {userId}");
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found during update attempt.");
                throw new NoSuchEntityException("User profile not found.");
            }

            // Use AutoMapper to update the existing user entity with the new data from the DTO
            _mapper.Map(updateProfileDTO, user);

            await _userRepository.Update(user);
            _logger.LogInformation($"Profile for user ID {userId} updated successfully.");

            return _mapper.Map<ReturnUserProfileDTO>(user);
        }
    }
}