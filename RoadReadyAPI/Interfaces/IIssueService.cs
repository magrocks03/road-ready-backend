using RoadReadyAPI.DTOs;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IIssueService
    {
        Task<ReturnIssueDTO> ReportIssueAsync(int userId, CreateIssueDTO createIssueDTO);
        Task<PagedResultDTO<ReturnIssueDTO>> GetUserIssuesAsync(int userId, PaginationDTO pagination);

        // --- CHANGE IS HERE ---
        Task<PagedResultDTO<ReturnIssueDTO>> GetAllIssuesAsync(PaginationDTO pagination);
    }
}
