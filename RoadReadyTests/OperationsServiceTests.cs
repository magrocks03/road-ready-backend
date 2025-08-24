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
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class OperationsServiceTests
    {
        private Mock<IVehicleRepository> _vehicleRepositoryMock;
        private Mock<IBookingRepository> _bookingRepositoryMock;
        private Mock<IIssueRepository> _issueRepositoryMock;
        private Mock<IBookingStatusRepository> _statusRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<OperationsService>> _loggerMock;
        private IOperationsService _operationsService;

        [SetUp]
        public void Setup()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _issueRepositoryMock = new Mock<IIssueRepository>();
            _statusRepositoryMock = new Mock<IBookingStatusRepository>();
            _loggerMock = new Mock<ILogger<OperationsService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OperationsMappingProfile());
                mc.AddProfile(new VehicleMappingProfile());
                mc.AddProfile(new BookingMappingProfile());
                mc.AddProfile(new IssueMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _operationsService = new OperationsService(
                _vehicleRepositoryMock.Object,
                _bookingRepositoryMock.Object,
                _issueRepositoryMock.Object,
                _statusRepositoryMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        #region GetAllBookingsAsync Tests

        [Test]
        public async Task GetAllBookingsAsync_WhenBookingsExist_ReturnsPaginatedResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var bookings = new List<Booking> { new Booking { Id = 1, Vehicle = new Vehicle(), Status = new BookingStatus(), BookingExtras = new List<BookingExtra>() } };

            _bookingRepositoryMock.Setup(r => r.GetTotalAllBookingsCountAsync()).ReturnsAsync(1);
            _bookingRepositoryMock.Setup(r => r.GetPagedAllBookingsAsync(pagination)).ReturnsAsync(bookings);

            // Act
            var result = await _operationsService.GetAllBookingsAsync(pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
        }

        #endregion

        #region GetAllIssuesAsync Tests

        [Test]
        public async Task GetAllIssuesAsync_WhenIssuesExist_ReturnsPaginatedResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var issues = new List<Issue> { new Issue { Id = 1, Booking = new Booking { User = new User(), Vehicle = new Vehicle() } } };

            _issueRepositoryMock.Setup(r => r.GetTotalAllIssuesCountAsync()).ReturnsAsync(1);
            _issueRepositoryMock.Setup(r => r.GetPagedAllIssuesAsync(pagination)).ReturnsAsync(issues);

            // Act
            var result = await _operationsService.GetAllIssuesAsync(pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
        }

        #endregion

        #region UpdateVehicleStatusAsync Tests

        [Test]
        public async Task UpdateVehicleStatusAsync_WhenVehicleExists_ReturnsUpdatedVehicleDTO()
        {
            // Arrange
            var vehicleId = 1;
            var updateDto = new UpdateVehicleStatusDTO { IsAvailable = false };
            var vehicle = new Vehicle { Id = vehicleId, IsAvailable = true };
            var updatedVehicleDetails = new Vehicle { Id = vehicleId, IsAvailable = false, Brand = new Brand(), Location = new Location() };

            _vehicleRepositoryMock.Setup(r => r.GetById(vehicleId)).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(r => r.Update(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(r => r.GetVehicleDetailsByIdAsync(vehicleId)).ReturnsAsync(updatedVehicleDetails);

            // Act
            var result = await _operationsService.UpdateVehicleStatusAsync(vehicleId, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsAvailable);
            _vehicleRepositoryMock.Verify(r => r.Update(It.Is<Vehicle>(v => !v.IsAvailable)), Times.Once);
        }

        [Test]
        public void UpdateVehicleStatusAsync_WhenVehicleDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var vehicleId = 99;
            var updateDto = new UpdateVehicleStatusDTO();
            _vehicleRepositoryMock.Setup(r => r.GetById(vehicleId)).ReturnsAsync((Vehicle)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _operationsService.UpdateVehicleStatusAsync(vehicleId, updateDto));
        }

        #endregion

        #region UpdateBookingStatusAsync Tests

        [Test]
        public async Task UpdateBookingStatusAsync_WhenBookingAndStatusExist_ReturnsUpdatedBookingDTO()
        {
            // Arrange
            var bookingId = 1;
            var updateDto = new UpdateBookingStatusDTO { StatusName = "Completed" };
            var booking = new Booking { Id = bookingId };
            var newStatus = new BookingStatus { Id = 3, Name = "Completed" };
            var updatedBookingDetails = new Booking { Id = bookingId, Status = newStatus, Vehicle = new Vehicle { Brand = new Brand(), Location = new Location() } };

            _bookingRepositoryMock.Setup(r => r.GetById(bookingId)).ReturnsAsync(booking);
            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync(updateDto.StatusName)).ReturnsAsync(newStatus);
            _bookingRepositoryMock.Setup(r => r.Update(It.IsAny<Booking>())).ReturnsAsync(booking);
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(bookingId)).ReturnsAsync(updatedBookingDetails);

            // Act
            var result = await _operationsService.UpdateBookingStatusAsync(bookingId, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Completed", result.Status);
        }

        [Test]
        public void UpdateBookingStatusAsync_WhenBookingDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var bookingId = 99;
            var updateDto = new UpdateBookingStatusDTO();
            _bookingRepositoryMock.Setup(r => r.GetById(bookingId)).ReturnsAsync((Booking)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _operationsService.UpdateBookingStatusAsync(bookingId, updateDto));
        }

        [Test]
        public void UpdateBookingStatusAsync_WhenStatusDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var bookingId = 1;
            var updateDto = new UpdateBookingStatusDTO { StatusName = "InvalidStatus" };
            _bookingRepositoryMock.Setup(r => r.GetById(bookingId)).ReturnsAsync(new Booking());
            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync(updateDto.StatusName)).ReturnsAsync((BookingStatus)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _operationsService.UpdateBookingStatusAsync(bookingId, updateDto));
        }

        #endregion

        #region UpdateIssueStatusAsync Tests

        [Test]
        public async Task UpdateIssueStatusAsync_WhenIssueExists_ReturnsUpdatedIssueDTO()
        {
            // Arrange
            var issueId = 1;
            var updateDto = new UpdateIssueStatusDTO { Status = "Resolved", AdminNotes = "Contacted customer." };
            var issue = new Issue { Id = issueId, Status = "Open" };
            var updatedIssueDetails = new Issue { Id = issueId, Status = "Resolved", AdminNotes = "Contacted customer.", Booking = new Booking { User = new User(), Vehicle = new Vehicle() } };

            _issueRepositoryMock.Setup(r => r.GetById(issueId)).ReturnsAsync(issue);
            _issueRepositoryMock.Setup(r => r.Update(It.IsAny<Issue>())).ReturnsAsync(issue);
            _issueRepositoryMock.Setup(r => r.GetIssueDetailsByIdAsync(issueId)).ReturnsAsync(updatedIssueDetails);

            // Act
            var result = await _operationsService.UpdateIssueStatusAsync(issueId, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Resolved", result.Status);
            Assert.AreEqual("Contacted customer.", result.AdminNotes);
        }

        [Test]
        public void UpdateIssueStatusAsync_WhenIssueDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var issueId = 99;
            var updateDto = new UpdateIssueStatusDTO();
            _issueRepositoryMock.Setup(r => r.GetById(issueId)).ReturnsAsync((Issue)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _operationsService.UpdateIssueStatusAsync(issueId, updateDto));
        }

        #endregion
    }
}