using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class RoleRepository : RepositoryDB<int, Role>, IRoleRepository
    {
        public RoleRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Role?> GetById(int key)
        {
            return await _context.Roles.SingleOrDefaultAsync(r => r.Id == key);
        }

        // --- IMPLEMENTATION OF THE NEW METHOD ---
        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}