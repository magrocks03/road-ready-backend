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
    public class ReviewRepository : RepositoryDB<int, Review>, IReviewRepository
    {
        public ReviewRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Review?> GetById(int key)
        {
            return await _context.Reviews.SingleOrDefaultAsync(r => r.Id == key);
        }

        public async Task<Review?> GetReviewDetailsByIdAsync(int reviewId)
        {
            return await _context.Reviews
                                 .Include(r => r.User)
                                 .Include(r => r.Vehicle)
                                 .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<List<Review>> GetPagedVehicleReviewsAsync(int vehicleId, PaginationDTO pagination)
        {
            return await _context.Reviews
                                 .Where(r => r.VehicleId == vehicleId)
                                 .Include(r => r.User)
                                 .Include(r => r.Vehicle)
                                 .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                                 .Take(pagination.PageSize)
                                 .ToListAsync();
        }

        public async Task<int> GetVehicleReviewsCountAsync(int vehicleId)
        {
            return await _context.Reviews.CountAsync(r => r.VehicleId == vehicleId);
        }

        // --- IMPLEMENTATION OF NEW METHOD ---
        public async Task<List<Review>> GetAllReviewsByVehicleIdAsync(int vehicleId)
        {
            return await _context.Reviews
                                 .Where(r => r.VehicleId == vehicleId)
                                 .ToListAsync();
        }
    }
}
