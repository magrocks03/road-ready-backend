using RoadReadyAPI.Models;

namespace RoadReadyAPI.Interfaces
{
    public interface ILocationRepository : IRepository<int, Location>
    {
        // Currently, no location-specific methods are needed.
        // We can add them here later if required.
    }
}