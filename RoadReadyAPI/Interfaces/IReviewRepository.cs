using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IReviewRepository : IRepository<int, Review>
    {
        Task<Review?> GetReviewDetailsByIdAsync(int reviewId);
        Task<List<Review>> GetPagedVehicleReviewsAsync(int vehicleId, PaginationDTO pagination);
        Task<int> GetVehicleReviewsCountAsync(int vehicleId);
        // --- NEW METHOD ADDED HERE ---
        Task<List<Review>> GetAllReviewsByVehicleIdAsync(int vehicleId);
    }
}
