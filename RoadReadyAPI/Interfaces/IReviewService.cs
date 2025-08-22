using RoadReadyAPI.DTOs;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IReviewService
    {
        Task<ReturnReviewDTO> AddReviewAsync(int userId, CreateReviewDTO createReviewDTO);

        Task<PagedResultDTO<ReturnReviewDTO>> GetVehicleReviewsAsync(int vehicleId, PaginationDTO pagination);
    }
}