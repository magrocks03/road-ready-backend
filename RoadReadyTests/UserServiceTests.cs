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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private IMapper _mapper;
        private Mock<ILogger<UserService>> _loggerMock;
        private IUserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<UserService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserMappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _userRoleRepositoryMock.Object,
                _tokenServiceMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        #region Login Tests

        [Test]
        public async Task Login_WithValidCredentials_ReturnsUserDTO()
        {
            // Arrange
            var loginDto = new LoginUserDTO { Email = "test@example.com", Password = "password123" };
            var hmac = new HMACSHA512();
            var user = new User
            {
                Id = 1,
                Email = loginDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)),
                PasswordHashKey = hmac.Key,
                UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = "Customer" } } }
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(loginDto.Email)).ReturnsAsync(user);
            _tokenServiceMock.Setup(s => s.GenerateToken(It.IsAny<User>())).Returns("fake_jwt_token");

            // Act
            var result = await _userService.Login(loginDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual("fake_jwt_token", result.Token);
        }

        [Test]
        public void Login_WithNonExistentUser_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var loginDto = new LoginUserDTO { Email = "nouser@example.com", Password = "password123" };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(loginDto.Email)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.Login(loginDto));
        }

        [Test]
        public void Login_WithInvalidPassword_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var loginDto = new LoginUserDTO { Email = "test@example.com", Password = "wrong_password" };
            var hmac = new HMACSHA512();
            var user = new User
            {
                Id = 1,
                Email = loginDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("correct_password")),
                PasswordHashKey = hmac.Key,
                UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = "Customer" } } }
            };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(loginDto.Email)).ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userService.Login(loginDto));
        }

        #endregion

        #region Register Tests

        [Test]
        public async Task Register_WithNewUser_ReturnsUserDTO()
        {
            // Arrange
            var registerDto = new RegisterUserDTO { Email = "newuser@example.com", Password = "password123", FirstName = "New", LastName = "User" };
            var customerRole = new Role { Id = 2, Name = "Customer" };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(registerDto.Email)).ReturnsAsync((User)null);
            _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("Customer")).ReturnsAsync(customerRole);
            _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User u) => { u.Id = 1; return u; });
            _userRoleRepositoryMock.Setup(r => r.Add(It.IsAny<UserRole>())).ReturnsAsync(new UserRole());
            _tokenServiceMock.Setup(s => s.GenerateToken(It.IsAny<User>())).Returns("new_user_token");

            // Act
            var result = await _userService.Register(registerDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(registerDto.Email, result.Email);
            Assert.AreEqual("Customer", result.Role);
            Assert.AreEqual("new_user_token", result.Token);
        }

        [Test]
        public void Register_WithExistingEmail_ThrowsUserAlreadyExistsException()
        {
            // Arrange
            var registerDto = new RegisterUserDTO { Email = "existing@example.com", Password = "password123" };
            var existingUser = new User { Id = 1, Email = "existing@example.com" };

            _userRepositoryMock.Setup(r => r.GetUserByEmail(registerDto.Email)).ReturnsAsync(existingUser);

            // Act & Assert
            Assert.ThrowsAsync<UserAlreadyExistsException>(() => _userService.Register(registerDto));
        }

        #endregion
    }
}