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
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private Mock<IBookingRepository> _bookingRepositoryMock;
        private Mock<IRefundRepository> _refundRepositoryMock;
        private Mock<IBookingStatusRepository> _statusRepositoryMock;
        private Mock<IIssueService> _issueServiceMock; // <-- FIX IS HERE
        private IMapper _mapper;
        private Mock<ILogger<AdminService>> _loggerMock;
        private IAdminService _adminService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _refundRepositoryMock = new Mock<IRefundRepository>();
            _statusRepositoryMock = new Mock<IBookingStatusRepository>();
            _issueServiceMock = new Mock<IIssueService>(); // <-- FIX IS HERE
            _loggerMock = new Mock<ILogger<AdminService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AdminMappingProfile());
                mc.AddProfile(new BookingMappingProfile());
                mc.AddProfile(new IssueMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _adminService = new AdminService(
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _userRoleRepositoryMock.Object,
                _bookingRepositoryMock.Object,
                _refundRepositoryMock.Object,
                _statusRepositoryMock.Object,
                _issueServiceMock.Object, // <-- FIX IS HERE
                _mapper,
                _loggerMock.Object
            );
        }

        #region GetAllUsersAsync Tests
        [Test]
        public async Task GetAllUsersAsync_WhenUsersExist_ReturnsPaginatedResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var users = new List<User> { new User { Id = 1, UserRoles = new List<UserRole>() } };
            _userRepositoryMock.Setup(r => r.GetTotalUserCountAsync()).ReturnsAsync(1);
            _userRepositoryMock.Setup(r => r.GetPagedUsersAsync(pagination)).ReturnsAsync(users);

            // Act
            var result = await _adminService.GetAllUsersAsync(pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
        }
        #endregion

        #region GetAllBookingsAsync Tests
        [Test]
        public async Task GetAllBookingsAsync_WhenCalled_ReturnsPaginatedResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var bookings = new List<Booking> { new Booking { Id = 1, Vehicle = new Vehicle(), Status = new BookingStatus(), BookingExtras = new List<BookingExtra>() } };
            _bookingRepositoryMock.Setup(r => r.GetTotalAllBookingsCountAsync()).ReturnsAsync(1);
            _bookingRepositoryMock.Setup(r => r.GetPagedAllBookingsAsync(pagination)).ReturnsAsync(bookings);

            // Act
            var result = await _adminService.GetAllBookingsAsync(pagination);

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
            var issues = new PagedResultDTO<ReturnIssueDTO> { Items = new List<ReturnIssueDTO> { new ReturnIssueDTO() }, TotalCount = 1 };
            _issueServiceMock.Setup(s => s.GetAllIssuesAsync(pagination)).ReturnsAsync(issues);

            // Act
            var result = await _adminService.GetAllIssuesAsync(pagination);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
        }
        #endregion

        #region UpdateUserRoleAsync Tests
        [Test]
        public async Task UpdateUserRoleAsync_WhenValid_UpdatesRoleAndReturnsUser()
        {
            // Arrange
            var updateUserDto = new UpdateUserRoleDTO { UserId = 2, RoleName = "Rental Agent" };
            var user = new User { Id = 2 };
            var role = new Role { Id = 3, Name = "Rental Agent" };
            var existingRoles = new List<UserRole> { new UserRole { UserId = 2, RoleId = 2 } }.AsQueryable();
            var finalUser = new User { Id = 2, UserRoles = new List<UserRole> { new UserRole { Role = role } } };


            _userRepositoryMock.Setup(r => r.GetById(updateUserDto.UserId)).ReturnsAsync(user);
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync(updateUserDto.RoleName)).ReturnsAsync(role);
            _userRoleRepositoryMock.Setup(r => r.GetAll()).Returns(existingRoles);
            _userRoleRepositoryMock.Setup(r => r.Delete(It.IsAny<int>())).ReturnsAsync(new UserRole());
            _userRoleRepositoryMock.Setup(r => r.Add(It.IsAny<UserRole>())).ReturnsAsync(new UserRole());
            _userRepositoryMock.Setup(r => r.GetAllUsersWithRolesAsync()).ReturnsAsync(new List<User> { finalUser });

            // Act
            var result = await _adminService.UpdateUserRoleAsync(updateUserDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Rental Agent", result.Roles.First());
        }

        [Test]
        public void UpdateUserRoleAsync_WhenUserNotFound_ThrowsNoSuchEntityException()
        {
            // Arrange
            var updateUserDto = new UpdateUserRoleDTO();
            _userRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _adminService.UpdateUserRoleAsync(updateUserDto));
        }
        #endregion

        #region GetDashboardStatsAsync Tests
        [Test]
        public async Task GetDashboardStatsAsync_WhenDataExists_ReturnsCorrectStats()
        {
            // Arrange
            var popularVehicle1 = new Vehicle { Id = 1, Name = "Innova" };
            var popularVehicle2 = new Vehicle { Id = 2, Name = "Mustang" };

            var allBookings = new List<Booking>
    {
        // 2 completed bookings for the Innova
        new Booking { VehicleId = 1, Status = new BookingStatus { Name = "Completed" }, TotalCost = 1000 },
        new Booking { VehicleId = 1, Status = new BookingStatus { Name = "Completed" }, TotalCost = 1500 },
        // 1 completed booking for the Mustang
        new Booking { VehicleId = 2, Status = new BookingStatus { Name = "Completed" }, TotalCost = 2000 },
        // 1 booking that is not completed
        new Booking { VehicleId = 1, Status = new BookingStatus { Name = "Confirmed" }, TotalCost = 500 }
    };

            // We now mock all the repository methods with our detailed test data
            _userRepositoryMock.Setup(r => r.GetTotalUserCountAsync()).ReturnsAsync(5);
            _bookingRepositoryMock.Setup(r => r.GetTotalBookingCountAsync()).ReturnsAsync(4);
            _bookingRepositoryMock.Setup(r => r.GetTotalRevenueAsync()).ReturnsAsync(4500m); // Only completed bookings
            _bookingRepositoryMock.Setup(r => r.GetMostPopularVehiclesAsync(5)).ReturnsAsync(new List<Vehicle> { popularVehicle1, popularVehicle2 });
            _bookingRepositoryMock.Setup(r => r.GetAllBookingsWithDetailsAsync()).ReturnsAsync(allBookings);

            // Act
            var result = await _adminService.GetDashboardStatsAsync();

            // Assert
            // Verify all the stats
            Assert.AreEqual(5, result.TotalUsers);
            Assert.AreEqual(4, result.TotalBookings);
            Assert.AreEqual(4500, result.TotalRevenue);

            // Verify the popular vehicle calculation
            Assert.AreEqual(2, result.MostPopularVehicles.Count);
            Assert.AreEqual(2, result.MostPopularVehicles.First(v => v.VehicleId == 1).BookingCount); // Innova should have 2 bookings
            Assert.AreEqual(1, result.MostPopularVehicles.First(v => v.VehicleId == 2).BookingCount); // Mustang should have 1 booking
        }
        #endregion

        #region ProcessRefundAsync Tests
        [Test]
        public async Task ProcessRefundAsync_WhenBookingIsCancelledAndEligible_ProcessesFullRefund()
        {
            // Arrange
            var adminUserId = 99;
            var processDto = new ProcessRefundDTO { BookingId = 1, Reason = "Customer complaint" }; // No amount specified
            var booking = new Booking { Id = 1, Status = new BookingStatus { Name = "Cancelled - Refund Pending" }, TotalCost = 5000 };
            var refundedStatus = new BookingStatus { Id = 5, Name = "Cancelled - Refunded" };
            var finalBooking = new Booking { Id = 1, Status = refundedStatus, Vehicle = new Vehicle { Location = new Location(), Brand = new Brand() }, BookingExtras = new List<BookingExtra>() };

            _bookingRepositoryMock.SetupSequence(r => r.GetBookingDetailsByIdAsync(processDto.BookingId))
                .ReturnsAsync(booking)
                .ReturnsAsync(finalBooking);

            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync("Cancelled - Refunded")).ReturnsAsync(refundedStatus);
            _refundRepositoryMock.Setup(r => r.Add(It.IsAny<Refund>())).ReturnsAsync(new Refund());
            _bookingRepositoryMock.Setup(r => r.Update(It.IsAny<Booking>())).ReturnsAsync(booking);

            // Act
            var result = await _adminService.ProcessRefundAsync(processDto, adminUserId);

            // Assert
            Assert.AreEqual("Cancelled - Refunded", result.Status);
            // Verify that the full amount was refunded
            _refundRepositoryMock.Verify(r => r.Add(It.Is<Refund>(rf => rf.Amount == 5000)), Times.Once);
        }

        [Test]
        public async Task ProcessRefundAsync_WhenBookingIsCompleted_ProcessesPartialRefund()
        {
            // Arrange
            var adminUserId = 99;
            // DTO specifies a partial refund amount
            var processDto = new ProcessRefundDTO { BookingId = 1, Reason = "Partial refund for issue", Amount = 1500 };
            var booking = new Booking { Id = 1, Status = new BookingStatus { Name = "Completed" }, TotalCost = 5000 };
            var refundedStatus = new BookingStatus { Id = 5, Name = "Cancelled - Refunded" };
            var finalBooking = new Booking { Id = 1, Status = refundedStatus, Vehicle = new Vehicle { Location = new Location(), Brand = new Brand() }, BookingExtras = new List<BookingExtra>() };

            _bookingRepositoryMock.SetupSequence(r => r.GetBookingDetailsByIdAsync(processDto.BookingId))
                .ReturnsAsync(booking)
                .ReturnsAsync(finalBooking);

            _statusRepositoryMock.Setup(r => r.GetStatusByNameAsync("Cancelled - Refunded")).ReturnsAsync(refundedStatus);
            _refundRepositoryMock.Setup(r => r.Add(It.IsAny<Refund>())).ReturnsAsync(new Refund());
            _bookingRepositoryMock.Setup(r => r.Update(It.IsAny<Booking>())).ReturnsAsync(booking);

            // Act
            var result = await _adminService.ProcessRefundAsync(processDto, adminUserId);

            // Assert
            Assert.AreEqual("Cancelled - Refunded", result.Status);
            // Verify that the partial amount was refunded
            _refundRepositoryMock.Verify(r => r.Add(It.Is<Refund>(rf => rf.Amount == 1500)), Times.Once);
        }

        [Test]
        public void ProcessRefundAsync_WhenBookingIsNotEligible_ThrowsAdminActionException()
        {
            // Arrange
            var processDto = new ProcessRefundDTO { BookingId = 1 };
            var booking = new Booking { Id = 1, Status = new BookingStatus { Name = "Confirmed" } }; // Not an eligible status
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(processDto.BookingId)).ReturnsAsync(booking);

            // Act & Assert
            Assert.ThrowsAsync<AdminActionException>(() => _adminService.ProcessRefundAsync(processDto, 1));
        }

        [Test]
        public void ProcessRefundAsync_WhenBookingNotFound_ThrowsNoSuchEntityException()
        {
            // Arrange
            var processDto = new ProcessRefundDTO { BookingId = 99 };
            _bookingRepositoryMock.Setup(r => r.GetBookingDetailsByIdAsync(processDto.BookingId)).ReturnsAsync((Booking)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _adminService.ProcessRefundAsync(processDto, 1));
        }
        #endregion

        #region CreateUserWithRoleAsync Tests
        [Test]
        public async Task CreateUserWithRoleAsync_WhenValid_CreatesUserAndReturnsDTO()
        {
            // Arrange
            var createDto = new AdminCreateUserDTO { Email = "newagent@example.com", Password = "Password123!", RoleName = "Rental Agent" };
            var role = new Role { Id = 3, Name = "Rental Agent" };
            var addedUser = new User { Id = 10, Email = createDto.Email };
            var finalUser = new User { Id = 10, Email = createDto.Email, UserRoles = new List<UserRole> { new UserRole { Role = role } } };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(createDto.Email)).ReturnsAsync((User)null);
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync(createDto.RoleName)).ReturnsAsync(role);
            _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(addedUser);
            _userRoleRepositoryMock.Setup(r => r.Add(It.IsAny<UserRole>())).ReturnsAsync(new UserRole());
            _userRepositoryMock.Setup(r => r.GetAllUsersWithRolesAsync()).ReturnsAsync(new List<User> { finalUser });

            // Act
            var result = await _adminService.CreateUserWithRoleAsync(createDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Rental Agent", result.Roles.First());
        }

        [Test]
        public void CreateUserWithRoleAsync_WhenRoleDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var createDto = new AdminCreateUserDTO { RoleName = "NonExistentRole" };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User)null);
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync(createDto.RoleName)).ReturnsAsync((Role)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _adminService.CreateUserWithRoleAsync(createDto));
        }
        #endregion

        #region DeactivateUserAsync Tests
        [Test]
        public async Task DeactivateUserAsync_WhenUserExists_RemovesRoles()
        {
            // Arrange
            var userId = 2;
            var user = new User { Id = userId };
            var existingRoles = new List<UserRole> { new UserRole { UserId = userId, RoleId = 2 } }.AsQueryable();
            var finalUser = new User { Id = userId, UserRoles = new List<UserRole>() };
            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
            _userRoleRepositoryMock.Setup(r => r.GetAll()).Returns(existingRoles);
            _userRoleRepositoryMock.Setup(r => r.Delete(userId)).ReturnsAsync(new UserRole());
            _userRepositoryMock.Setup(r => r.GetAllUsersWithRolesAsync()).ReturnsAsync(new List<User> { finalUser });

            // Act
            var result = await _adminService.DeactivateUserAsync(userId);

            // Assert
            Assert.IsEmpty(result.Roles);
        }
        #endregion
    }
}
