using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;

namespace RoadReadyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")] // All endpoints in this controller require the user to be a Customer.
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Initiates a new booking (Step 1 of 2).
        /// </summary>
        /// <param name="initiateBookingDTO">The details of the booking request.</param>
        /// <returns>A temporary booking ID and the total calculated cost.</returns>
        [HttpPost("initiate")]
        [ProducesResponseType(typeof(ReturnInitiateBookingDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnInitiateBookingDTO>> InitiateBooking(InitiateBookingDTO initiateBookingDTO)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var result = await _bookingService.InitiateBookingAsync(userId, initiateBookingDTO);
                return Ok(result);
            }
            catch (UserProfileIncompleteException ex)
            {
                _logger.LogWarning(ex, "Booking initiation failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Booking initiation failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (VehicleNotAvailableException ex)
            {
                _logger.LogWarning(ex, "Booking initiation failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Booking initiation failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while initiating a booking.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Confirms a booking after payment (Step 2 of 2).
        /// </summary>
        /// <param name="bookingId">The ID of the pending booking to confirm.</param>
        /// <param name="confirmPaymentDTO">The mock payment details.</param>
        /// <returns>The full details of the confirmed booking.</returns>
        [HttpPost("{bookingId}/confirm-payment")]
        [ProducesResponseType(typeof(ReturnBookingDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnBookingDTO>> ConfirmPayment(int bookingId, ConfirmPaymentDTO confirmPaymentDTO)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var result = await _bookingService.ConfirmBookingPaymentAsync(userId, bookingId, confirmPaymentDTO);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Payment confirmation failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Payment confirmation failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while confirming booking ID {bookingId}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a list of all bookings for the currently logged-in user.
        /// </summary>
        /// <returns>A list of the user's bookings.</returns>
        // --- CHANGE IS HERE ---
        [HttpGet("my-bookings")] // Changed back to [HttpGet]
        [ProducesResponseType(typeof(PagedResultDTO<ReturnBookingDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<ReturnBookingDTO>>> GetMyBookings([FromQuery] PaginationDTO pagination) 
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var bookings = await _bookingService.GetUserBookingsAsync(userId, pagination);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user bookings.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Cancels an upcoming booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to cancel.</param>
        /// <returns>The details of the cancelled booking.</returns>
        [HttpPut("{bookingId}/cancel")]
        [ProducesResponseType(typeof(ReturnBookingDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnBookingDTO>> CancelBooking(int bookingId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var result = await _bookingService.CancelBookingAsync(userId, bookingId);
                return Ok(result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Booking cancellation failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Booking cancellation failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while cancelling booking ID {bookingId}.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}