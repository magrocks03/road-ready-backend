using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Mappers;
using RoadReadyAPI.Models;
using RoadReadyAPI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private Mock<IBookingRepository> _bookingRepositoryMock;
        private Mock<IVehicleRepository> _vehicleRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<ReviewService>> _loggerMock;
        private IReviewService _reviewService;

        [SetUp]
        public void Setup()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _loggerMock = new Mock<ILogger<ReviewService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ReviewMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object,
                _bookingRepositoryMock.Object,
                _vehicleRepositoryMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        #region AddReviewAsync Tests

        [Test]
        public async Task AddReviewAsync_WhenBookingIsCompletedAndOwnedByUser_AddsReviewAndUpdatesRating()
        {
            // Arrange
            var userId = 1;
            var createDto = new CreateReviewDTO { BookingId = 1, Rating = 5 };
            var booking = new Booking { Id = 1, UserId = userId, VehicleId = 1, Status = new BookingStatus { Name = "Completed" } };
            var vehicle = new Vehicle { Id = 1 };
            var addedReview = new Review { Id = 101, Rating = 5 };
            var finalReview = new Review { Id = 101, User = new User(), Vehicle = new Vehicle() };

            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync(booking);
            _reviewRepositoryMock.Setup(r => r.Add(It.IsAny<Review>())).ReturnsAsync(addedReview);
            _vehicleRepositoryMock.Setup(r => r.GetById(booking.VehicleId)).ReturnsAsync(vehicle);
            _reviewRepositoryMock.Setup(r => r.GetAllReviewsByVehicleIdAsync(booking.VehicleId)).ReturnsAsync(new List<Review> { addedReview });
            _reviewRepositoryMock.Setup(r => r.GetReviewDetailsByIdAsync(addedReview.Id)).ReturnsAsync(finalReview);
            _vehicleRepositoryMock.Setup(r => r.Update(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);

            // Act
            var result = await _reviewService.AddReviewAsync(userId, createDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(101, result.ReviewId);
            _vehicleRepositoryMock.Verify(r => r.Update(It.Is<Vehicle>(v => v.AverageRating == 5)), Times.Once);
        }

        [Test]
        public void AddReviewAsync_WhenBookingNotFound_ThrowsNoSuchEntityException()
        {
            // Arrange
            var userId = 1;
            var createDto = new CreateReviewDTO { BookingId = 99 };
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync((Booking)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _reviewService.AddReviewAsync(userId, createDto));
        }

        [Test]
        public void AddReviewAsync_WhenBookingNotOwnedByUser_ThrowsReviewEligibilityException()
        {
            // Arrange
            var userId = 1; // Logged-in user
            var createDto = new CreateReviewDTO { BookingId = 1 };
            var booking = new Booking { Id = 1, UserId = 2 }; // Booking belongs to another user
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync(booking);

            // Act & Assert
            Assert.ThrowsAsync<ReviewEligibilityException>(() => _reviewService.AddReviewAsync(userId, createDto));
        }

        [Test]
        public void AddReviewAsync_WhenBookingIsNotCompleted_ThrowsReviewEligibilityException()
        {
            // Arrange
            var userId = 1;
            var createDto = new CreateReviewDTO { BookingId = 1 };
            var booking = new Booking { Id = 1, UserId = userId, Status = new BookingStatus { Name = "Confirmed" } }; // Not completed
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync(booking);

            // Act & Assert
            Assert.ThrowsAsync<ReviewEligibilityException>(() => _reviewService.AddReviewAsync(userId, createDto));
        }

        #endregion

        #region GetVehicleReviewsAsync Tests

        [Test]
        public async Task GetVehicleReviewsAsync_WhenReviewsExist_ReturnsPaginatedResult()
        {
            // Arrange
            var vehicleId = 1;
            var pagination = new PaginationDTO();
            var reviews = new List<Review> { new Review { User = new User(), Vehicle = new Vehicle() } };
            _reviewRepositoryMock.Setup(r => r.GetVehicleReviewsCountAsync(vehicleId)).ReturnsAsync(1);
            _reviewRepositoryMock.Setup(r => r.GetPagedVehicleReviewsAsync(vehicleId, pagination)).ReturnsAsync(reviews);

            // Act
            var result = await _reviewService.GetVehicleReviewsAsync(vehicleId, pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(1, result.TotalCount);
        }

        #endregion
    }
}