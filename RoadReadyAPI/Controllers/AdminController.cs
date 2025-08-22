using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RoadReadyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // All endpoints in this controller are restricted to Admins.
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Gets key statistics for the admin dashboard.
        /// </summary>
        /// <returns>An object containing dashboard statistics.</returns>
        [HttpGet("dashboard-stats")]
        [ProducesResponseType(typeof(AdminDashboardStatsDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<ActionResult<AdminDashboardStatsDTO>> GetDashboardStats()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating dashboard stats.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a paginated list of all users in the system.
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(PagedResultDTO<AdminReturnUserDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<AdminReturnUserDTO>>> GetAllUsers([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync(pagination);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all users.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Creates a new user with a specified role (e.g., Rental Agent).
        /// </summary>
        [HttpPost("users")]
        [ProducesResponseType(typeof(AdminReturnUserDTO), 201)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 409)]
        public async Task<ActionResult<AdminReturnUserDTO>> CreateUser(AdminCreateUserDTO createUserDTO)
        {
            try
            {
                var result = await _adminService.CreateUserWithRoleAsync(createUserDTO);
                return CreatedAtAction(nameof(GetAllUsers), result);
            }
            catch (UserAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, "User creation failed: {Message}", ex.Message);
                return Conflict(new ErrorModel(409, ex.Message));
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "User creation failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a user.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Deactivates a user's account by removing their roles.
        /// </summary>
        [HttpPut("users/{id}/deactivate")]
        [ProducesResponseType(typeof(AdminReturnUserDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<AdminReturnUserDTO>> DeactivateUser(int id)
        {
            try
            {
                var result = await _adminService.DeactivateUserAsync(id);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "User deactivation failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while deactivating user ID {id}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a list of all bookings in the system.
        /// </summary>
        /// <summary>
        /// Gets a paginated list of all bookings in the system.
        /// </summary>
        [HttpGet("bookings")]
        [ProducesResponseType(typeof(PagedResultDTO<ReturnBookingDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<ReturnBookingDTO>>> GetAllBookings([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var bookings = await _adminService.GetAllBookingsAsync(pagination);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all bookings.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a list of all issues reported by all users.
        /// </summary>
        [HttpGet("issues")]
        [ProducesResponseType(typeof(PagedResultDTO<ReturnIssueDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<ReturnIssueDTO>>> GetAllIssues([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var issues = await _adminService.GetAllIssuesAsync(pagination);
                return Ok(issues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all issues.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Updates the role of a specific user.
        /// </summary>
        [HttpPut("users/role")]
        [ProducesResponseType(typeof(AdminReturnUserDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<AdminReturnUserDTO>> UpdateUserRole(UpdateUserRoleDTO updateUserRoleDTO)
        {
            try
            {
                var result = await _adminService.UpdateUserRoleAsync(updateUserRoleDTO);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "User role update failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating a user role.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Processes a refund for a booking that is pending a refund.
        /// </summary>
        [HttpPost("refunds")]
        [ProducesResponseType(typeof(ReturnBookingDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnBookingDTO>> ProcessRefund(ProcessRefundDTO processRefundDTO)
        {
            try
            {
                var adminUserId = Convert.ToInt32(User.FindFirstValue("userId"));
                var result = await _adminService.ProcessRefundAsync(processRefundDTO, adminUserId);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Refund processing failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AdminActionException ex)
            {
                _logger.LogWarning(ex, "Refund processing failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing a refund.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}