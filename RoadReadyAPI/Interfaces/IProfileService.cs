using RoadReadyAPI.DTOs;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IProfileService
    {
        /// <summary>
        /// Gets the profile information for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile to retrieve.</param>
        /// <returns>The user's profile details.</returns>
        Task<ReturnUserProfileDTO> GetUserProfileAsync(int userId);

        /// <summary>
        /// Updates the profile information for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile to update.</param>
        /// <param name="updateProfileDTO">The new profile information.</param>
        /// <returns>The updated user's profile details.</returns>
        Task<ReturnUserProfileDTO> UpdateUserProfileAsync(int userId, UpdateUserProfileDTO updateProfileDTO);
    }
}
