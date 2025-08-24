using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System;
using System.Threading.Tasks;

namespace RoadReadyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Rental Agent")] // Endpoints are accessible to both Admins and Rental Agents.
    public class OperationsController : ControllerBase
    {
        private readonly IOperationsService _operationsService;
        private readonly ILogger<OperationsController> _logger;

        public OperationsController(IOperationsService operationsService, ILogger<OperationsController> logger)
        {
            _operationsService = operationsService;
            _logger = logger;
        }

        // --- NEW ENDPOINTS ADDED HERE ---

        /// <summary>
        /// Gets a paginated list of all bookings in the system.
        /// </summary>
        [HttpGet("bookings")]
        [ProducesResponseType(typeof(PagedResultDTO<ReturnBookingDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<ReturnBookingDTO>>> GetAllBookings([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var bookings = await _operationsService.GetAllBookingsAsync(pagination);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all bookings for operations.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a paginated list of all issues reported by all users.
        /// </summary>
        [HttpGet("issues")]
        [ProducesResponseType(typeof(PagedResultDTO<ReturnIssueDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<ReturnIssueDTO>>> GetAllIssues([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var issues = await _operationsService.GetAllIssuesAsync(pagination);
                return Ok(issues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all issues for operations.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Updates the availability status of a vehicle (e.g., for maintenance).
        /// </summary>
        /// <param name="id">The ID of the vehicle to update.</param>
        /// <param name="updateStatusDTO">The new availability status.</param>
        /// <returns>The updated vehicle's details.</returns>
        [HttpPut("vehicles/{id}/status")]
        [ProducesResponseType(typeof(ReturnVehicleDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnVehicleDTO>> UpdateVehicleStatus(int id, UpdateVehicleStatusDTO updateStatusDTO)
        {
            try
            {
                var result = await _operationsService.UpdateVehicleStatusAsync(id, updateStatusDTO);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Vehicle status update failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while updating status for vehicle ID {id}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Updates the status of a booking (e.g., for check-in/check-out).
        /// </summary>
        /// <param name="id">The ID of the booking to update.</param>
        /// <param name="updateStatusDTO">The new booking status.</param>
        /// <returns>The updated booking's details.</returns>
        [HttpPut("bookings/{id}/status")]
        [ProducesResponseType(typeof(ReturnBookingDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnBookingDTO>> UpdateBookingStatus(int id, UpdateBookingStatusDTO updateStatusDTO)
        {
            try
            {
                var result = await _operationsService.UpdateBookingStatusAsync(id, updateStatusDTO);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Booking status update failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while updating status for booking ID {id}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Updates the status and notes of a reported issue.
        /// </summary>
        /// <param name="id">The ID of the issue to update.</param>
        /// <param name="updateStatusDTO">The new status and optional notes.</param>
        /// <returns>The updated issue's details.</returns>
        [HttpPut("issues/{id}/status")]
        [ProducesResponseType(typeof(ReturnIssueDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnIssueDTO>> UpdateIssueStatus(int id, UpdateIssueStatusDTO updateStatusDTO)
        {
            try
            {
                var result = await _operationsService.UpdateIssueStatusAsync(id, updateStatusDTO);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Issue status update failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while updating status for issue ID {id}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}