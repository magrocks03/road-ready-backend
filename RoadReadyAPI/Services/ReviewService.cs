using AutoMapper;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepository,
            IBookingRepository bookingRepository,
            IVehicleRepository vehicleRepository,
            IMapper mapper,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _bookingRepository = bookingRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnReviewDTO> AddReviewAsync(int userId, CreateReviewDTO createReviewDTO)
        {
            _logger.LogInformation($"User {userId} attempting to add a review for booking {createReviewDTO.BookingId}");

            var booking = await _bookingRepository.GetBookingDetailsByIdAsync(createReviewDTO.BookingId);
            if (booking == null)
            {
                throw new NoSuchEntityException("Booking not found.");
            }
            if (booking.UserId != userId)
            {
                throw new ReviewEligibilityException("You can only review your own bookings.");
            }
            if (booking.Status.Name != "Completed")
            {
                throw new ReviewEligibilityException("You can only review a booking after it has been completed.");
            }

            var newReview = _mapper.Map<Review>(createReviewDTO);
            newReview.UserId = userId;
            newReview.VehicleId = booking.VehicleId;
            newReview.ReviewDate = System.DateTime.UtcNow;

            var addedReview = await _reviewRepository.Add(newReview);

            await UpdateVehicleAverageRating(booking.VehicleId);

            var fullReview = await _reviewRepository.GetReviewDetailsByIdAsync(addedReview.Id);

            _logger.LogInformation($"Review {addedReview.Id} added successfully for vehicle {booking.VehicleId}.");
            return _mapper.Map<ReturnReviewDTO>(fullReview);
        }

        public async Task<PagedResultDTO<ReturnReviewDTO>> GetVehicleReviewsAsync(int vehicleId, PaginationDTO pagination)
        {
            _logger.LogInformation($"Fetching paginated reviews for vehicle ID {vehicleId}");

            var totalCount = await _reviewRepository.GetVehicleReviewsCountAsync(vehicleId);
            var reviews = await _reviewRepository.GetPagedVehicleReviewsAsync(vehicleId, pagination);

            var reviewDtos = _mapper.Map<List<ReturnReviewDTO>>(reviews);

            return new PagedResultDTO<ReturnReviewDTO>
            {
                Items = reviewDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        private async Task UpdateVehicleAverageRating(int vehicleId)
        {
            // --- FIX IS HERE ---
            // We now call the new, correct repository method.
            var reviewsForVehicle = await _reviewRepository.GetAllReviewsByVehicleIdAsync(vehicleId);

            if (reviewsForVehicle.Any())
            {
                var averageRating = reviewsForVehicle.Average(r => r.Rating);
                var vehicle = await _vehicleRepository.GetById(vehicleId);
                if (vehicle != null)
                {
                    vehicle.AverageRating = averageRating;
                    await _vehicleRepository.Update(vehicle);
                    _logger.LogInformation($"Updated average rating for vehicle {vehicleId} to {averageRating}");
                }
            }
        }
    }
}