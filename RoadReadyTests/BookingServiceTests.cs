using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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

namespace RoadReadyTests
{
    [TestFixture]
    public class BookingServiceTests
    {
        private Mock<IBookingRepository> _bookingRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IVehicleRepository> _vehicleRepositoryMock;
        private Mock<IExtraRepository> _extraRepositoryMock;
        private Mock<IPaymentRepository> _paymentRepositoryMock;
        private Mock<IBookingStatusRepository> _statusRepositoryMock;
        private Mock<IBookingExtraRepository> _bookingExtraRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<BookingService>> _loggerMock;
        private IBookingService _bookingService;

        [SetUp]
        public void Setup()
        {
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _extraRepositoryMock = new Mock<IExtraRepository>();
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _statusRepositoryMock = new Mock<IBookingStatusRepository>();
            _bookingExtraRepositoryMock = new Mock<IBookingExtraRepository>();
            _loggerMock = new Mock<ILogger<BookingService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BookingMappingProfile());
                mc.AddProfile(new VehicleMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _bookingService = new BookingService(
                _bookingRepositoryMock.Object,
                _userRepositoryMock.Object,
                _vehicleRepositoryMock.Object,
                _extraRepositoryMock.Object,
                _paymentRepositoryMock.Object,
                _statusRepositoryMock.Object,
                _bookingExtraRepositoryMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        #region InitiateBookingAsync Tests

        [Test]
        public async Task InitiateBookingAsync_WhenValid_ReturnsInitiateBookingDTO()
        {
            // Arrange
            var userId = 1;
            var initiateDto = new InitiateBookingDTO { VehicleId = 1, StartDate = DateTime.UtcNow.AddDays(2), EndDate = DateTime.UtcNow.AddDays(4) };
            var user = new User { Id = userId, Address = "123 Main St" };
            var vehicle = new Vehicle { Id = 1, IsAvailable = true, PricePerDay = 1000 };
            var pendingStatus = new BookingStatus { Id = 1, Name = "Pending" };

            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
            _vehicleRepositoryMock.Setup(r => r.GetById(initiateDto.VehicleId)).ReturnsAsync(vehicle);
            _bookingRepositoryMock.Setup(r => r.IsVehicleBookedForDateRangeAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);
            _extraRepositoryMock.Setup(r => r.GetExtrasByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Extra>());
            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync("Pending")).ReturnsAsync(pendingStatus);
            _bookingRepositoryMock.Setup(r => r.Add(It.IsAny<Booking>())).ReturnsAsync(new Booking { Id = 101, TotalCost = 2000 });

            // Act
            var result = await _bookingService.InitiateBookingAsync(userId, initiateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(101, result.BookingId);
        }

        #endregion

        #region Extras Cost Calc Tests
        [Test]
        public async Task InitiateBookingAsync_WithExtras_CalculatesCorrectTotalCost()
        {
            // Arrange
            var userId = 1;
            var initiateDto = new InitiateBookingDTO
            {
                VehicleId = 1,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(4), // 2 days rental
                ExtraIds = new List<int> { 1, 2 }
            };

            var user = new User { Id = userId, Address = "123 Main St" };
            var vehicle = new Vehicle { Id = 1, IsAvailable = true, PricePerDay = 1000 };
            var pendingStatus = new BookingStatus { Id = 1, Name = "Pending" };

            var extras = new List<Extra>
{
new Extra { Id = 1, Price = 500, PriceType = PriceType.FlatFee },
new Extra { Id = 2, Price = 250, PriceType = PriceType.PerDay }
};

            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
            _vehicleRepositoryMock.Setup(r => r.GetById(initiateDto.VehicleId)).ReturnsAsync(vehicle);
            _bookingRepositoryMock.Setup(r => r.IsVehicleBookedForDateRangeAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);

            // --- FIX IS HERE ---
            // We now mock our new, simple method.
            _extraRepositoryMock.Setup(r => r.GetExtrasByIdsAsync(initiateDto.ExtraIds)).ReturnsAsync(extras);

            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync("Pending")).ReturnsAsync(pendingStatus);
            _bookingRepositoryMock.Setup(r => r.Add(It.IsAny<Booking>())).ReturnsAsync((Booking b) => b);

            // Act
            var result = await _bookingService.InitiateBookingAsync(userId, initiateDto);

            // Assert
            Assert.AreEqual(3000, result.TotalCost);
        }
        #endregion

        #region ConfirmBookingPaymentAsync Tests

        [Test]
        public async Task ConfirmBookingPaymentAsync_WhenBookingIsPending_ReturnsConfirmedBookingDTO()
        {
            // Arrange
            var userId = 1;
            var bookingId = 101;
            var paymentDto = new ConfirmPaymentDTO();
            var pendingBooking = new Booking { Id = bookingId, UserId = userId, Status = new BookingStatus { Name = "Pending" }, TotalCost = 2000 };
            var confirmedStatus = new BookingStatus { Id = 2, Name = "Confirmed" };
            var finalBooking = new Booking { Id = bookingId, Status = confirmedStatus, Vehicle = new Vehicle { Location = new Location(), Brand = new Brand() }, BookingExtras = new List<BookingExtra>(), Payment = new Payment() };

            _bookingRepositoryMock.SetupSequence(r => r.GetBookingDetailsByIdAsync(bookingId))
                .ReturnsAsync(pendingBooking)
                .ReturnsAsync(finalBooking);

            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync("Confirmed")).ReturnsAsync(confirmedStatus);
            _paymentRepositoryMock.Setup(r => r.Add(It.IsAny<Payment>())).ReturnsAsync(new Payment());
            _bookingRepositoryMock.Setup(r => r.Update(It.IsAny<Booking>())).ReturnsAsync(pendingBooking);

            // Act
            var result = await _bookingService.ConfirmBookingPaymentAsync(userId, bookingId, paymentDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Confirmed", result.Status);
        }

        #endregion

        #region DatesInvalid/AlreadyBooked Tests

        [Test]
        public void InitiateBookingAsync_WhenDatesAreInvalid_ThrowsValidationException()
        {
            // Arrange
            var userId = 1;
            // Start date is after the end date, which is invalid
            var initiateDto = new InitiateBookingDTO { VehicleId = 1, StartDate = DateTime.UtcNow.AddDays(4), EndDate = DateTime.UtcNow.AddDays(2) };
            var user = new User { Id = userId, Address = "123 Main St" };
            var vehicle = new Vehicle { Id = 1, IsAvailable = true };

            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
            _vehicleRepositoryMock.Setup(r => r.GetById(initiateDto.VehicleId)).ReturnsAsync(vehicle);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _bookingService.InitiateBookingAsync(userId, initiateDto));
        }

        [Test]
        public void InitiateBookingAsync_WhenVehicleIsAlreadyBooked_ThrowsVehicleNotAvailableException()
        {
            // Arrange
            var userId = 1;
            var initiateDto = new InitiateBookingDTO { VehicleId = 1, StartDate = DateTime.UtcNow.AddDays(2), EndDate = DateTime.UtcNow.AddDays(4) };
            var user = new User { Id = userId, Address = "123 Main St" };
            var vehicle = new Vehicle { Id = 1, IsAvailable = true };

            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
            _vehicleRepositoryMock.Setup(r => r.GetById(initiateDto.VehicleId)).ReturnsAsync(vehicle);
            // Simulate that the repository found an overlapping booking
            _bookingRepositoryMock.Setup(r => r.IsVehicleBookedForDateRangeAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);

            // Act & Assert
            Assert.ThrowsAsync<VehicleNotAvailableException>(() => _bookingService.InitiateBookingAsync(userId, initiateDto));
        }

        #endregion

        #region GetUserBookingsAsync Tests (Updated for Pagination)

        [Test]
        public async Task GetUserBookingsAsync_WhenBookingsExist_ReturnsPaginatedResult()
        {
            // Arrange
            var userId = 1;
            var pagination = new PaginationDTO { PageNumber = 1, PageSize = 10 };
            var bookings = new List<Booking> { new Booking { Id = 1, Vehicle = new Vehicle(), Status = new BookingStatus(), BookingExtras = new List<BookingExtra>() } };

            _bookingRepositoryMock.Setup(r => r.GetUserBookingsCountAsync(userId)).ReturnsAsync(1);
            _bookingRepositoryMock.Setup(r => r.GetPagedUserBookingsAsync(userId, pagination)).ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetUserBookingsAsync(userId, pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
        }

        #endregion

        #region CancelBookingAsync Tests

        [Test]
        public async Task CancelBookingAsync_WhenEligibleForRefund_ReturnsBookingWithRefundPendingStatus()
        {
            // Arrange
            var userId = 1;
            var bookingId = 101;
            var bookingToCancel = new Booking { Id = bookingId, UserId = userId, StartDate = DateTime.UtcNow.AddDays(3), Status = new BookingStatus { Name = "Confirmed" }, Vehicle = new Vehicle { Location = new Location(), Brand = new Brand() }, BookingExtras = new List<BookingExtra>() };
            var refundStatus = new BookingStatus { Id = 4, Name = "Cancelled - Refund Pending" };
            var finalCancelledBooking = new Booking { Id = bookingId, UserId = userId, Status = refundStatus, Vehicle = new Vehicle { Location = new Location(), Brand = new Brand() }, BookingExtras = new List<BookingExtra>() };

            _bookingRepositoryMock.SetupSequence(r => r.GetBookingDetailsByIdAsync(bookingId))
                .ReturnsAsync(bookingToCancel)
                .ReturnsAsync(finalCancelledBooking);

            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync("Cancelled - Refund Pending")).ReturnsAsync(refundStatus);
            _bookingRepositoryMock.Setup(r => r.Update(It.IsAny<Booking>())).ReturnsAsync(bookingToCancel);

            // Act
            var result = await _bookingService.CancelBookingAsync(userId, bookingId);

            // Assert
            Assert.AreEqual("Cancelled - Refund Pending", result.Status);
        }

        // --- NEW EXCEPTION TESTS ADDED HERE ---

        [Test]
        public void CancelBookingAsync_WhenBookingNotFound_ThrowsNoSuchEntityException()
        {
            // Arrange
            var userId = 1;
            var bookingId = 99;
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(bookingId)).ReturnsAsync((Booking)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _bookingService.CancelBookingAsync(userId, bookingId));
        }

        [Test]
        public void CancelBookingAsync_WhenBookingIsNotConfirmed_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var bookingId = 101;
            var bookingToCancel = new Booking { Id = bookingId, UserId = userId, Status = new BookingStatus { Name = "Pending" } }; // Not confirmed
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(bookingId)).ReturnsAsync(bookingToCancel);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CancelBookingAsync(userId, bookingId));
        }

        [Test]
        public async Task CancelBookingAsync_WhenNotEligibleForRefund_ReturnsBookingWithNoRefundStatus()
        {
            // Arrange
            var userId = 1;
            var bookingId = 101;
            var bookingToCancel = new Booking { Id = bookingId, UserId = userId, StartDate = DateTime.UtcNow.AddHours(1), Status = new BookingStatus { Name = "Confirmed" } }; // Too late to cancel
            var noRefundStatus = new BookingStatus { Id = 5, Name = "Cancelled - No Refund" };
            var finalCancelledBooking = new Booking { Id = bookingId, UserId = userId, Status = noRefundStatus, Vehicle = new Vehicle { Location = new Location(), Brand = new Brand() }, BookingExtras = new List<BookingExtra>() };

            _bookingRepositoryMock.SetupSequence(r => r.GetBookingDetailsByIdAsync(bookingId))
                .ReturnsAsync(bookingToCancel)
                .ReturnsAsync(finalCancelledBooking);

            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync("Cancelled - No Refund")).ReturnsAsync(noRefundStatus);
            _bookingRepositoryMock.Setup(r => r.Update(It.IsAny<Booking>())).ReturnsAsync(bookingToCancel);

            // Act
            var result = await _bookingService.CancelBookingAsync(userId, bookingId);

            // Assert
            Assert.AreEqual("Cancelled - No Refund", result.Status);
        }

        #endregion
    }
}