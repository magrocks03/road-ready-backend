using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IVehicleRepository : IRepository<int, Vehicle>
    {
        Task<Vehicle?> GetVehicleDetailsByIdAsync(int vehicleId);

        // --- NEW METHODS FOR SIMPLE PAGINATION ---
        Task<List<Vehicle>> GetPagedVehiclesAsync(VehicleSearchCriteriaDTO criteria);
        Task<int> GetTotalVehicleCountAsync(VehicleSearchCriteriaDTO criteria);
    }
}
