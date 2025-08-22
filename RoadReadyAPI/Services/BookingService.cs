using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IExtraRepository _extraRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingStatusRepository _statusRepository;
        private readonly IBookingExtraRepository _bookingExtraRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            IVehicleRepository vehicleRepository,
            IExtraRepository extraRepository,
            IPaymentRepository paymentRepository,
            IBookingStatusRepository statusRepository,
            IBookingExtraRepository bookingExtraRepository,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _vehicleRepository = vehicleRepository;
            _extraRepository = extraRepository;
            _paymentRepository = paymentRepository;
            _statusRepository = statusRepository;
            _bookingExtraRepository = bookingExtraRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnInitiateBookingDTO> InitiateBookingAsync(int userId, InitiateBookingDTO initiateBookingDTO)
        {
            _logger.LogInformation($"Initiating booking for user ID {userId}");

            // 1. Validate User and Vehicle
            var user = await _userRepository.GetById(userId);
            if (user == null) throw new NoSuchEntityException("User not found.");
            if (string.IsNullOrEmpty(user.Address)) throw new UserProfileIncompleteException();

            var vehicle = await _vehicleRepository.GetById(initiateBookingDTO.VehicleId);
            if (vehicle == null) throw new NoSuchEntityException("The selected vehicle does not exist.");
            if (!vehicle.IsAvailable) throw new VehicleNotAvailableException("This vehicle is currently unavailable (e.g., under maintenance).");

            // 2. Check for date validity and vehicle availability
            if (initiateBookingDTO.StartDate >= initiateBookingDTO.EndDate || initiateBookingDTO.StartDate < DateTime.UtcNow)
            {
                throw new ValidationException("Invalid booking dates provided.");
            }
            if (await _bookingRepository.IsVehicleBookedForDateRangeAsync(vehicle.Id, initiateBookingDTO.StartDate, initiateBookingDTO.EndDate))
            {
                throw new VehicleNotAvailableException("Vehicle is already booked for the selected dates.");
            }

            // 3. Calculate Total Cost
            var rentalDays = (initiateBookingDTO.EndDate - initiateBookingDTO.StartDate).TotalDays;
            decimal totalCost = (decimal)rentalDays * vehicle.PricePerDay;

            var selectedExtras = new List<Extra>();
            if (initiateBookingDTO.ExtraIds != null && initiateBookingDTO.ExtraIds.Any())
            {
                selectedExtras = await _extraRepository.GetExtrasByIdsAsync(initiateBookingDTO.ExtraIds);

                foreach (var extra in selectedExtras)
                {
                    totalCost += extra.PriceType == PriceType.PerDay ? extra.Price * (decimal)rentalDays : extra.Price;
                }
            }

            // 4. Create Pending Booking
            var pendingStatus = await _statusRepository.GetStatusByNameAsync("Pending");
            if (pendingStatus == null) throw new InvalidOperationException("Booking status 'Pending' not found.");

            var newBooking = new Booking
            {
                UserId = userId,
                VehicleId = vehicle.Id,
                StartDate = initiateBookingDTO.StartDate,
                EndDate = initiateBookingDTO.EndDate,
                TotalCost = Math.Round(totalCost, 2),
                StatusId = pendingStatus.Id,
                BookingDate = DateTime.UtcNow
            };
            var pendingBooking = await _bookingRepository.Add(newBooking);

            // 5. Add Selected Extras to the Booking
            foreach (var extra in selectedExtras)
            {
                await _bookingExtraRepository.Add(new BookingExtra { BookingId = pendingBooking.Id, ExtraId = extra.Id });
            }

            _logger.LogInformation($"Booking ID {pendingBooking.Id} initiated successfully with a total cost of {pendingBooking.TotalCost}.");
            return new ReturnInitiateBookingDTO { BookingId = pendingBooking.Id, TotalCost = pendingBooking.TotalCost };
        }

        public async Task<ReturnBookingDTO> ConfirmBookingPaymentAsync(int userId, int bookingId, ConfirmPaymentDTO confirmPaymentDTO)
        {
            _logger.LogInformation($"Confirming payment for booking ID {bookingId} by user ID {userId}");

            var booking = await _bookingRepository.GetBookingDetailsByIdAsync(bookingId);
            if (booking == null || booking.UserId != userId)
            {
                throw new NoSuchEntityException("Booking not found or you do not have permission to access it.");
            }
            if (booking.Status.Name != "Pending")
            {
                throw new InvalidOperationException("This booking is not pending and cannot be confirmed.");
            }

            // Simulate payment success
            var newPayment = new Payment
            {
                BookingId = bookingId,
                Amount = booking.TotalCost,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Simulated Card Payment",
                TransactionStatus = "Succeeded"
            };
            await _paymentRepository.Add(newPayment);

            // Update booking status to "Confirmed"
            var confirmedStatus = await _statusRepository.GetStatusByNameAsync("Confirmed");
            if (confirmedStatus == null) throw new InvalidOperationException("Booking status 'Confirmed' not found.");

            booking.StatusId = confirmedStatus.Id;
            await _bookingRepository.Update(booking);

            _logger.LogInformation($"Booking ID {bookingId} confirmed successfully.");
            var confirmedBookingDetails = await _bookingRepository.GetBookingDetailsByIdAsync(bookingId);
            return _mapper.Map<ReturnBookingDTO>(confirmedBookingDetails);
        }

        // --- METHOD UPDATED ---
        public async Task<PagedResultDTO<ReturnBookingDTO>> GetUserBookingsAsync(int userId, PaginationDTO pagination)
        {
            _logger.LogInformation($"Fetching paginated bookings for user ID {userId}");

            var totalCount = await _bookingRepository.GetUserBookingsCountAsync(userId);
            var bookings = await _bookingRepository.GetPagedUserBookingsAsync(userId, pagination);

            var bookingDtos = _mapper.Map<List<ReturnBookingDTO>>(bookings);

            return new PagedResultDTO<ReturnBookingDTO>
            {
                Items = bookingDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<ReturnBookingDTO> CancelBookingAsync(int userId, int bookingId)
        {
            _logger.LogInformation($"Attempting to cancel booking ID {bookingId} for user ID {userId}");

            var booking = await _bookingRepository.GetBookingDetailsByIdAsync(bookingId);
            if (booking == null || booking.UserId != userId)
            {
                throw new NoSuchEntityException("Booking not found or you do not have permission to access it.");
            }
            if (booking.Status.Name != "Confirmed")
            {
                throw new InvalidOperationException("Only confirmed bookings can be cancelled.");
            }

            // Business Rule: Cancellation policy (e.g., 48 hours)
            var cancellationStatusName = (booking.StartDate - DateTime.UtcNow).TotalHours > 48
                ? "Cancelled - Refund Pending"
                : "Cancelled - No Refund";

            var cancellationStatus = await _statusRepository.GetStatusByNameAsync(cancellationStatusName);
            if (cancellationStatus == null) throw new InvalidOperationException($"Booking status '{cancellationStatusName}' not found.");

            booking.StatusId = cancellationStatus.Id;
            await _bookingRepository.Update(booking);

            _logger.LogInformation($"Booking ID {bookingId} cancelled. New status: {cancellationStatusName}.");
            var cancelledBookingDetails = await _bookingRepository.GetBookingDetailsByIdAsync(bookingId);
            return _mapper.Map<ReturnBookingDTO>(cancelledBookingDetails);
        }
    }
}