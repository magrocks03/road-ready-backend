using RoadReadyAPI.DTOs;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IPasswordService
    {
        /// <summary>
        /// Handles a forgot password request by generating and saving a reset token for a user.
        /// </summary>
        /// <param name="forgotPasswordDTO">The DTO containing the user's email.</param>
        /// <returns>A boolean indicating if the operation was successful from a technical standpoint.</returns>
        Task<bool> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);

        /// <summary>
        /// Resets a user's password using a valid reset token.
        /// </summary>
        /// <param name="resetPasswordDTO">The DTO containing the token and the new password.</param>
        /// <returns>A boolean indicating if the password was successfully reset.</returns>
        Task<bool> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
    }
}
