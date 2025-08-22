using Microsoft.Extensions.Logging;
using RoadReadyAPI.Interfaces;
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            _logger.LogInformation("--- SIMULATING EMAIL ---");
            _logger.LogInformation($"To: {toEmail}");
            _logger.LogInformation("Subject: Reset Your RoadReady Password");
            _logger.LogInformation("Body: Please reset your password by clicking the following link.");
            _logger.LogInformation($"Reset Link: {resetLink}");
            _logger.LogInformation("--------------------------");

            return Task.CompletedTask;
        }
    }
}