using RoadReadyAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IBookingService
    {
        /// <summary>
        /// Initiates a new booking, checks for availability, calculates the cost, and creates a pending booking.
        /// </summary>
        /// <param name="userId">The ID of the user making the booking.</param>
        /// <param name="initiateBookingDTO">The details of the booking request.</param>
        /// <returns>A DTO with the temporary booking ID and the total cost.</returns>
        Task<ReturnInitiateBookingDTO> InitiateBookingAsync(int userId, InitiateBookingDTO initiateBookingDTO);

        /// <summary>
        /// Confirms a pending booking after a successful (simulated) payment.
        /// </summary>
        /// <param name="userId">The ID of the user confirming the booking.</param>
        /// <param name="bookingId">The ID of the pending booking to confirm.</param>
        /// <param name="confirmPaymentDTO">The mock payment details.</param>
        /// <returns>The full details of the confirmed booking.</returns>
        Task<ReturnBookingDTO> ConfirmBookingPaymentAsync(int userId, int bookingId, ConfirmPaymentDTO confirmPaymentDTO);

        Task<PagedResultDTO<ReturnBookingDTO>> GetUserBookingsAsync(int userId, PaginationDTO pagination);

        /// <summary>
        /// Cancels an upcoming booking for a user.
        /// </summary>
        /// <param name="userId">The ID of the user cancelling the booking.</param>
        /// <param name="bookingId">The ID of the booking to cancel.</param>
        /// <returns>The details of the cancelled booking.</returns>
        Task<ReturnBookingDTO> CancelBookingAsync(int userId, int bookingId);
    }
}