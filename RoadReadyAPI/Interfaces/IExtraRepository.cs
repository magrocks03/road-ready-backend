using RoadReadyAPI.Models;

namespace RoadReadyAPI.Interfaces
{
    public interface IExtraRepository : IRepository<int, Extra>
    {
        // --- NEW METHOD ADDED HERE ---
        Task<List<Extra>> GetExtrasByIdsAsync(List<int> extraIds);
    }
}