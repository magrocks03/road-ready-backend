using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<PasswordService> _logger;

        public PasswordService(IUserRepository userRepository, IEmailService emailService, ILogger<PasswordService> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            _logger.LogInformation($"Forgot password request received for email: {forgotPasswordDTO.Email}");

            // --- CHANGE IS HERE ---
            var user = await _userRepository.GetUserByEmail(forgotPasswordDTO.Email);

            if (user == null)
            {
                _logger.LogWarning($"Password reset requested for non-existent email: {forgotPasswordDTO.Email}");
                return true;
            }

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).Replace('+', '-').Replace('/', '_');
            user.PasswordResetToken = token;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);
            await _userRepository.Update(user);

            var resetLink = $"http://localhost:5173/reset-password?token={token}";
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

            _logger.LogInformation($"Password reset token generated for user: {user.Email}");
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            _logger.LogInformation("Attempting to reset password with token.");

            // --- CHANGE IS HERE ---
            var user = await _userRepository.GetUserByPasswordResetTokenAsync(resetPasswordDTO.Token);

            if (user == null || user.ResetTokenExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired password reset token provided.");
                throw new InvalidCredentialsException("Invalid or expired password reset token.");
            }

            using var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resetPasswordDTO.NewPassword));
            user.PasswordHashKey = hmac.Key;
            user.PasswordResetToken = null;
            user.ResetTokenExpiresAt = null;
            await _userRepository.Update(user);

            _logger.LogInformation($"Password successfully reset for user: {user.Email}");
            return true;
        }
    }
}