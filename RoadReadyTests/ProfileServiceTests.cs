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
    public class ProfileServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<ProfileService>> _loggerMock;
        private IProfileService _profileService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<ProfileService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProfileMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _profileService = new ProfileService(
                _userRepositoryMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        #region GetUserProfileAsync Tests

        [Test]
        public async Task GetUserProfileAsync_WhenUserExists_ReturnsUserProfileDTO()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Address = "123 Main St"
            };
            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);

            // Act
            var result = await _profileService.GetUserProfileAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.Address, result.Address);
        }

        [Test]
        public void GetUserProfileAsync_WhenUserDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var userId = 99;
            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _profileService.GetUserProfileAsync(userId));
        }

        #endregion

        #region UpdateUserProfileAsync Tests

        [Test]
        public async Task UpdateUserProfileAsync_WhenUserExists_ReturnsUpdatedProfileDTO()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Address = "Old Address"
            };
            var updateDto = new UpdateUserProfileDTO
            {
                FirstName = "Jonathan",
                LastName = "Doe",
                Address = "New Address"
            };

            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

            // Act
            var result = await _profileService.UpdateUserProfileAsync(userId, updateDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updateDto.FirstName, result.FirstName);
            Assert.AreEqual(updateDto.Address, result.Address);
            // Verify that the Update method on the repository was called exactly once.
            _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public void UpdateUserProfileAsync_WhenUserDoesNotExist_ThrowsNoSuchEntityException()
        {
            // Arrange
            var userId = 99;
            var updateDto = new UpdateUserProfileDTO();
            _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchEntityException>(() => _profileService.UpdateUserProfileAsync(userId, updateDto));
        }

        #endregion
    }
}