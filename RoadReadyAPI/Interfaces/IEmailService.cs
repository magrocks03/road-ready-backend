using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}