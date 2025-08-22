using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Controllers
{
    [Route("api")]
    [ApiController]
    [AllowAnonymous] // All endpoints in this controller are public.
    public class HelperController : ControllerBase
    {
        private readonly IHelperService _helperService;
        private readonly ILogger<HelperController> _logger;

        public HelperController(IHelperService helperService, ILogger<HelperController> logger)
        {
            _helperService = helperService;
            _logger = logger;
        }

        /// <summary>
        /// Gets a list of all car brands.
        /// </summary>
        /// <returns>A list of car brands.</returns>
        [HttpGet("brands")]
        [ProducesResponseType(typeof(List<ReturnBrandDTO>), 200)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<ActionResult<List<ReturnBrandDTO>>> GetAllBrands()
        {
            try
            {
                var brands = await _helperService.GetAllBrandsAsync();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all brands.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a list of all rental locations.
        /// </summary>
        /// <returns>A list of rental locations.</returns>
        [HttpGet("locations")]
        [ProducesResponseType(typeof(List<ReturnLocationDTO>), 200)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<ActionResult<List<ReturnLocationDTO>>> GetAllLocations()
        {
            try
            {
                var locations = await _helperService.GetAllLocationsAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all locations.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a list of all available optional extras.
        /// </summary>
        /// <returns>A list of optional extras.</returns>
        [HttpGet("extras")]
        [ProducesResponseType(typeof(List<ReturnExtraDTO>), 200)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<ActionResult<List<ReturnExtraDTO>>> GetAllExtras()
        {
            try
            {
                var extras = await _helperService.GetAllExtrasAsync();
                return Ok(extras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all extras.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}