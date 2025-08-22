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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class IssueServiceTests
    {
        private Mock<IIssueRepository> _issueRepositoryMock;
        private Mock<IBookingRepository> _bookingRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<IssueService>> _loggerMock;
        private IIssueService _issueService;

        [SetUp]
        public void Setup()
        {
            _issueRepositoryMock = new Mock<IIssueRepository>();
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _loggerMock = new Mock<ILogger<IssueService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new IssueMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _issueService = new IssueService(
                _issueRepositoryMock.Object,
                _bookingRepositoryMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        #region ReportIssueAsync Tests

        [Test]
        public async Task ReportIssueAsync_WhenBookingIsActiveAndOwnedByUser_ReportsIssueSuccessfully()
        {
            // Arrange
            var userId = 1;
            var createDto = new CreateIssueDTO { BookingId = 1, IssueDescription = "The car has a flat tire." };
            var booking = new Booking
            {
                Id = 1,
                UserId = userId,
                StartDate = DateTime.UtcNow.AddHours(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            var addedIssue = new Issue { Id = 101 };
            var finalIssue = new Issue { Id = 101, Booking = new Booking { User = new User(), Vehicle = new Vehicle() } };

            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync(booking);
            _issueRepositoryMock.Setup(r => r.Add(It.IsAny<Issue>())).ReturnsAsync(addedIssue);
            _issueRepositoryMock.Setup(r => r.GetIssueDetailsByIdAsync(addedIssue.Id)).ReturnsAsync(finalIssue);

            // Act
            var result = await _issueService.ReportIssueAsync(userId, createDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(101, result.IssueId);
        }

        [Test]
        public void ReportIssueAsync_WhenBookingNotFound_ThrowsNoSuchEntityException()
        {
            // Arrange
            var userId = 1;
            var createDto = new CreateIssueDTO { BookingId = 99 };
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync((Booking)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _issueService.ReportIssueAsync(userId, createDto));
        }

        [Test]
        public void ReportIssueAsync_WhenBookingNotOwnedByUser_ThrowsIssueReportingException()
        {
            // Arrange
            var userId = 1; // Logged-in user
            var createDto = new CreateIssueDTO { BookingId = 1 };
            var booking = new Booking { Id = 1, UserId = 2 }; // Booking belongs to another user
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync(booking);

            // Act & Assert
            Assert.ThrowsAsync<IssueReportingException>(() => _issueService.ReportIssueAsync(userId, createDto));
        }

        [Test]
        public void ReportIssueAsync_WhenBookingHasNotStarted_ThrowsIssueReportingException()
        {
            // Arrange
            var userId = 1;
            var createDto = new CreateIssueDTO { BookingId = 1 };
            var booking = new Booking { Id = 1, UserId = userId, StartDate = DateTime.UtcNow.AddDays(1) }; // Booking starts tomorrow
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync(booking);

            // Act & Assert
            Assert.ThrowsAsync<IssueReportingException>(() => _issueService.ReportIssueAsync(userId, createDto));
        }

        [Test]
        public void ReportIssueAsync_WhenBookingHasEnded_ThrowsIssueReportingException()
        {
            // Arrange
            var userId = 1;
            var createDto = new CreateIssueDTO { BookingId = 1 };
            var booking = new Booking { Id = 1, UserId = userId, StartDate = DateTime.UtcNow.AddDays(-3), EndDate = DateTime.UtcNow.AddDays(-1) }; // Booking ended yesterday
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(createDto.BookingId)).ReturnsAsync(booking);

            // Act & Assert
            Assert.ThrowsAsync<IssueReportingException>(() => _issueService.ReportIssueAsync(userId, createDto));
        }

        #endregion

        #region GetUserIssuesAsync Tests

        [Test]
        public async Task GetUserIssuesAsync_WhenIssuesExistForUser_ReturnsPaginatedResult()
        {
            // Arrange
            var userId = 1;
            var pagination = new PaginationDTO();
            var user = new User { FirstName = "Steve", LastName = "Rogers" };
            var issues = new List<Issue>
            {
                new Issue { Id = 1, Booking = new Booking { UserId = userId, User = user, Vehicle = new Vehicle() } }
            };

            _issueRepositoryMock.Setup(r => r.GetUserIssuesCountAsync(userId)).ReturnsAsync(1);
            _issueRepositoryMock.Setup(r => r.GetPagedUserIssuesAsync(userId, pagination)).ReturnsAsync(issues);

            // Act
            var result = await _issueService.GetUserIssuesAsync(userId, pagination);

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
            var result = await _issueService.GetAllIssuesAsync(pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
        }

        #endregion
    }
}
