using RoadReadyAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IAdminService
    {
        Task<PagedResultDTO<AdminReturnUserDTO>> GetAllUsersAsync(PaginationDTO pagination);
        // --- CHANGE IS HERE ---
        Task<PagedResultDTO<ReturnBookingDTO>> GetAllBookingsAsync(PaginationDTO pagination);
        Task<AdminReturnUserDTO> UpdateUserRoleAsync(UpdateUserRoleDTO updateUserRoleDTO);
        Task<ReturnBookingDTO> ProcessRefundAsync(ProcessRefundDTO processRefundDTO, int adminUserId);
        Task<PagedResultDTO<ReturnIssueDTO>> GetAllIssuesAsync(PaginationDTO pagination);
        Task<AdminReturnUserDTO> CreateUserWithRoleAsync(AdminCreateUserDTO createUserDTO);
        Task<AdminReturnUserDTO> DeactivateUserAsync(int userId);

        // --- NEW METHOD ADDED HERE ---

        /// <summary>
        /// Generates a report with key dashboard statistics.
        /// </summary>
        /// <returns>An object containing dashboard statistics.</returns>
        Task<AdminDashboardStatsDTO> GetDashboardStatsAsync();
    }
}
