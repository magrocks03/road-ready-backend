using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class IssueRepository : RepositoryDB<int, Issue>, IIssueRepository
    {
        public IssueRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Issue?> GetById(int key)
        {
            return await _context.Issues.SingleOrDefaultAsync(i => i.Id == key);
        }

        public async Task<Issue?> GetIssueDetailsByIdAsync(int issueId)
        {
            return await _context.Issues
                                 .Include(i => i.Booking).ThenInclude(b => b.User)
                                 .Include(i => i.Booking).ThenInclude(b => b.Vehicle)
                                 .FirstOrDefaultAsync(i => i.Id == issueId);
        }

        public async Task<List<Issue>> GetPagedUserIssuesAsync(int userId, PaginationDTO pagination)
        {
            return await _context.Issues
                                 .Where(i => i.Booking.UserId == userId)
                                 .Include(i => i.Booking).ThenInclude(b => b.User)
                                 .Include(i => i.Booking).ThenInclude(b => b.Vehicle)
                                 .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                 .Take(pagination.PageSize)
                                 .ToListAsync();
        }

        public async Task<int> GetUserIssuesCountAsync(int userId)
        {
            return await _context.Issues.CountAsync(i => i.Booking.UserId == userId);
        }

        // --- IMPLEMENTATION OF NEW METHODS ---
        public async Task<List<Issue>> GetPagedAllIssuesAsync(PaginationDTO pagination)
        {
            return await _context.Issues
                                 .Include(i => i.Booking).ThenInclude(b => b.User)
                                 .Include(i => i.Booking).ThenInclude(b => b.Vehicle)
                                 .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                 .Take(pagination.PageSize)
                                 .ToListAsync();
        }

        public async Task<int> GetTotalAllIssuesCountAsync()
        {
            return await _context.Issues.CountAsync();
        }
    }
}