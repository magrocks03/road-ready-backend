using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RoadReadyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _logger = logger;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ReturnUserProfileDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnUserProfileDTO>> GetMyProfile()
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var profile = await _profileService.GetUserProfileAsync(userId);
                return Ok(profile);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning($"Profile not found: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user profile.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        [HttpPut("me")]
        [ProducesResponseType(typeof(ReturnUserProfileDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnUserProfileDTO>> UpdateMyProfile(UpdateUserProfileDTO updateProfileDTO)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var updatedProfile = await _profileService.UpdateUserProfileAsync(userId, updateProfileDTO);
                return Ok(updatedProfile);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning($"Profile update failed: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user profile.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}
