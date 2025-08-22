using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class UserRoleRepository : RepositoryDB<int, UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(RoadReadyContext context) : base(context) { }

        public override async Task<UserRole?> GetById(int key)
        {
            // Note: This gets the first role assignment for a given UserId.
            return await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == key);
        }
    }
}