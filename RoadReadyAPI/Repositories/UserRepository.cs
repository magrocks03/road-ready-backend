using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Repositories
{
    public class UserRepository : RepositoryDB<int, User>, IUserRepository
    {
        public UserRepository(RoadReadyContext context) : base(context) { }

        public override async Task<User?> GetById(int key)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Id == key);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                                 .Include(u => u.UserRoles)
                                 .ThenInclude(ur => ur.Role)
                                 .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByPasswordResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public async Task<List<User>> GetAllUsersWithRolesAsync()
        {
            return await _context.Users
                                 .Include(u => u.UserRoles)
                                 .ThenInclude(ur => ur.Role)
                                 .ToListAsync();
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        // --- IMPLEMENTATION OF NEW METHOD ---
        public async Task<List<User>> GetPagedUsersAsync(PaginationDTO pagination)
        {
            return await _context.Users
                                 .Include(u => u.UserRoles)
                                 .ThenInclude(ur => ur.Role)
                                 .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                 .Take(pagination.PageSize)
                                 .ToListAsync();
        }
    }
}