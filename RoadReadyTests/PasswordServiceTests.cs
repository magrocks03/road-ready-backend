using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using RoadReadyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RoadReadyTests
{
    [TestFixture]
    public class PasswordServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ILogger<PasswordService>> _loggerMock;
        private IPasswordService _passwordService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<PasswordService>>();

            _passwordService = new PasswordService(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _loggerMock.Object
            );
        }

        #region ForgotPasswordAsync Tests

        [Test]
        public async Task ForgotPasswordAsync_WhenUserExists_GeneratesTokenAndSendsEmail()
        {
            // Arrange
            var forgotPasswordDto = new ForgotPasswordDTO { Email = "test@example.com" };
            var user = new User { Email = forgotPasswordDto.Email };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(forgotPasswordDto.Email)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).ReturnsAsync(user);

            // Act
            var result = await _passwordService.ForgotPasswordAsync(forgotPasswordDto);

            // Assert
            Assert.IsTrue(result);
            _userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.PasswordResetToken != null)), Times.Once);
            _emailServiceMock.Verify(s => s.SendPasswordResetEmailAsync(user.Email, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ForgotPasswordAsync_WhenUserDoesNotExist_ReturnsTrueAndDoesNotSendEmail()
        {
            // Arrange
            var forgotPasswordDto = new ForgotPasswordDTO { Email = "nouser@example.com" };
            _userRepositoryMock.Setup(r => r.GetUserByEmail(forgotPasswordDto.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _passwordService.ForgotPasswordAsync(forgotPasswordDto);

            // Assert
            Assert.IsTrue(result);
            _emailServiceMock.Verify(s => s.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region ResetPasswordAsync Tests

        [Test]
        public async Task ResetPasswordAsync_WithValidToken_ResetsPasswordSuccessfully()
        {
            // Arrange
            // --- CHANGE IS HERE --- Using a strong password
            var resetDto = new ResetPasswordDTO { Token = "valid_token", NewPassword = "NewPassword123!" };
            var user = new User { Email = "test@example.com", PasswordResetToken = "valid_token", ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(10) };
            _userRepositoryMock.Setup(r => r.GetUserByPasswordResetTokenAsync(resetDto.Token)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).ReturnsAsync(user);

            // Act
            var result = await _passwordService.ResetPasswordAsync(resetDto);

            // Assert
            Assert.IsTrue(result);
            _userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.PasswordResetToken == null)), Times.Once);
        }

        [Test]
        public void ResetPasswordAsync_WithInvalidToken_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var resetDto = new ResetPasswordDTO { Token = "invalid_token", NewPassword = "NewPassword123!" };
            _userRepositoryMock.Setup(r => r.GetUserByPasswordResetTokenAsync(resetDto.Token)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _passwordService.ResetPasswordAsync(resetDto));
        }

        [Test]
        public void ResetPasswordAsync_WithExpiredToken_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var resetDto = new ResetPasswordDTO { Token = "expired_token", NewPassword = "NewPassword123!" };
            var user = new User { Email = "test@example.com", PasswordResetToken = "expired_token", ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(-10) }; // Expired
            _userRepositoryMock.Setup(r => r.GetUserByPasswordResetTokenAsync(resetDto.Token)).ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _passwordService.ResetPasswordAsync(resetDto));
        }

        #endregion
    }
}