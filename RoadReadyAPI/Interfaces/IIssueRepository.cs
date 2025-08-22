using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IIssueRepository : IRepository<int, Issue>
    {
        Task<Issue?> GetIssueDetailsByIdAsync(int issueId);
        Task<List<Issue>> GetPagedUserIssuesAsync(int userId, PaginationDTO pagination);
        Task<int> GetUserIssuesCountAsync(int userId);

        // --- NEW METHODS ADDED HERE ---
        Task<List<Issue>> GetPagedAllIssuesAsync(PaginationDTO pagination);
        Task<int> GetTotalAllIssuesCountAsync();
    }
}
