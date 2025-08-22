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
    [Route("api/")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new review for a completed booking. Restricted to Customers.
        /// </summary>
        /// <param name="createReviewDTO">The details of the review.</param>
        /// <returns>The newly created review's details.</returns>
        [HttpPost("reviews")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ReturnReviewDTO), 201)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnReviewDTO>> AddReview(CreateReviewDTO createReviewDTO)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var result = await _reviewService.AddReviewAsync(userId, createReviewDTO);
                return CreatedAtAction(nameof(GetVehicleReviews), new { vehicleId = result.VehicleId }, result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Review creation failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (ReviewEligibilityException ex)
            {
                _logger.LogWarning(ex, "Review creation failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a review.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets all reviews for a specific vehicle. This endpoint is public.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle.</param>
        /// <returns>A list of reviews for the specified vehicle.</returns>
        [HttpGet("vehicles/{vehicleId}/reviews")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResultDTO<ReturnReviewDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<ReturnReviewDTO>>> GetVehicleReviews(int vehicleId, [FromQuery] PaginationDTO pagination)
        {
            try
            {
                var reviews = await _reviewService.GetVehicleReviewsAsync(vehicleId, pagination);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching reviews for vehicle ID {vehicleId}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}