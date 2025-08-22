using System.Collections.Generic;
using System.Threading.Tasks;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Interfaces
{
    public interface IUserRepository : IRepository<int, User>
    {
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByPasswordResetTokenAsync(string token);
        // --- NEW METHOD ADDED HERE ---
        Task<List<User>> GetAllUsersWithRolesAsync();
        // --- NEW METHOD ADDED HERE ---
        Task<int> GetTotalUserCountAsync();
        // --- NEW METHOD ADDED HERE ---
        Task<List<User>> GetPagedUsersAsync(PaginationDTO pagination);
    }
}