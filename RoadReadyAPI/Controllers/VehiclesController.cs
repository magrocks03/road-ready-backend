using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<ReturnVehicleDTO>>> GetAllVehicles([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var criteria = new VehicleSearchCriteriaDTO { PageNumber = pagination.PageNumber, PageSize = pagination.PageSize };
                var vehicles = await _vehicleService.SearchVehiclesAsync(criteria);
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all vehicles.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        [HttpPost("search")]
        public async Task<ActionResult<PagedResultDTO<ReturnVehicleDTO>>> SearchVehicles([FromBody] VehicleSearchCriteriaDTO criteria)
        {
            try
            {
                var vehicles = await _vehicleService.SearchVehiclesAsync(criteria);
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for vehicles.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReturnVehicleDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnVehicleDTO>> GetVehicleById(int id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                return Ok(vehicle);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching vehicle with ID {id}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnVehicleDTO), 201)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<ActionResult<ReturnVehicleDTO>> AddVehicle(CreateVehicleDTO createVehicleDTO)
        {
            try
            {
                var result = await _vehicleService.AddVehicleAsync(createVehicleDTO);
                return CreatedAtAction(nameof(GetVehicleById), new { id = result.Id }, result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a new vehicle.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ReturnVehicleDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnVehicleDTO>> UpdateVehicle(int id, UpdateVehicleDTO updateVehicleDTO)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicleDetailsAsync(id, updateVehicleDTO);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating vehicle with ID {id}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult> DeleteVehicle(int id)
        {
            try
            {
                await _vehicleService.DeleteVehicleAsync(id);
                return Ok(new { message = $"Vehicle with ID {id} deleted successfully." });
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting vehicle with ID {id}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}