using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IRoleRepository : IRepository<int, Role>
    {
        // --- NEW METHOD ADDED HERE ---
        Task<Role?> GetRoleByNameAsync(string roleName);
    }
}
