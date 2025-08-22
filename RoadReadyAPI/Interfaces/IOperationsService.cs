using RoadReadyAPI.DTOs;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IOperationsService
    {
        /// <summary>
        /// Updates the availability status of a specific vehicle.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle to update.</param>
        /// <param name="updateStatusDTO">The DTO containing the new availability status.</param>
        /// <returns>The updated vehicle's details.</returns>
        Task<ReturnVehicleDTO> UpdateVehicleStatusAsync(int vehicleId, UpdateVehicleStatusDTO updateStatusDTO);

        /// <summary>
        /// Updates the status of a specific booking (e.g., for check-in/check-out).
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="updateStatusDTO">The DTO containing the new status name.</param>
        /// <returns>The updated booking's details.</returns>
        Task<ReturnBookingDTO> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusDTO updateStatusDTO);

        /// <summary>
        /// Updates the status and notes of a reported issue.
        /// </summary>
        /// <param name="issueId">The ID of the issue to update.</param>
        /// <param name="updateStatusDTO">The DTO containing the new status and admin notes.</param>
        /// <returns>The updated issue's details.</returns>
        Task<ReturnIssueDTO> UpdateIssueStatusAsync(int issueId, UpdateIssueStatusDTO updateStatusDTO);
    }
}