using AutoMapper;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class OperationsService : IOperationsService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IBookingStatusRepository _statusRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OperationsService> _logger;

        public OperationsService(
            IVehicleRepository vehicleRepository,
            IBookingRepository bookingRepository,
            IIssueRepository issueRepository,
            IBookingStatusRepository statusRepository,
            IMapper mapper,
            ILogger<OperationsService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _bookingRepository = bookingRepository;
            _issueRepository = issueRepository;
            _statusRepository = statusRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnVehicleDTO> UpdateVehicleStatusAsync(int vehicleId, UpdateVehicleStatusDTO updateStatusDTO)
        {
            _logger.LogInformation($"Agent/Admin updating status for vehicle ID: {vehicleId}");
            var vehicle = await _vehicleRepository.GetById(vehicleId);
            if (vehicle == null)
            {
                throw new NoSuchEntityException("Vehicle not found.");
            }

            vehicle.IsAvailable = updateStatusDTO.IsAvailable;
            await _vehicleRepository.Update(vehicle);

            var updatedVehicleDetails = await _vehicleRepository.GetVehicleDetailsByIdAsync(vehicleId);
            _logger.LogInformation($"Vehicle ID {vehicleId} availability updated to {vehicle.IsAvailable}.");
            return _mapper.Map<ReturnVehicleDTO>(updatedVehicleDetails);
        }

        public async Task<ReturnBookingDTO> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusDTO updateStatusDTO)
        {
            _logger.LogInformation($"Agent/Admin updating status for booking ID: {bookingId}");
            var booking = await _bookingRepository.GetById(bookingId);
            if (booking == null)
            {
                throw new NoSuchEntityException("Booking not found.");
            }

            var newStatus = await _statusRepository.GetStatusByNameAsync(updateStatusDTO.StatusName);
            if (newStatus == null)
            {
                throw new NoSuchEntityException($"The status '{updateStatusDTO.StatusName}' is not a valid booking status.");
            }

            booking.StatusId = newStatus.Id;
            await _bookingRepository.Update(booking);

            var updatedBookingDetails = await _bookingRepository.GetBookingDetailsByIdAsync(bookingId);
            _logger.LogInformation($"Booking ID {bookingId} status updated to '{newStatus.Name}'.");
            return _mapper.Map<ReturnBookingDTO>(updatedBookingDetails);
        }

        public async Task<ReturnIssueDTO> UpdateIssueStatusAsync(int issueId, UpdateIssueStatusDTO updateStatusDTO)
        {
            _logger.LogInformation($"Agent/Admin updating status for issue ID: {issueId}");
            var issue = await _issueRepository.GetById(issueId);
            if (issue == null)
            {
                throw new NoSuchEntityException("Issue not found.");
            }

            issue.Status = updateStatusDTO.Status;
            if (updateStatusDTO.AdminNotes != null)
            {
                issue.AdminNotes = updateStatusDTO.AdminNotes;
            }

            await _issueRepository.Update(issue);

            var updatedIssueDetails = await _issueRepository.GetIssueDetailsByIdAsync(issueId);
            _logger.LogInformation($"Issue ID {issueId} status updated to '{issue.Status}'.");
            return _mapper.Map<ReturnIssueDTO>(updatedIssueDetails);
        }
    }
}
