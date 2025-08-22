using RoadReadyAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IVehicleService
    {
        Task<ReturnVehicleDTO> AddVehicleAsync(CreateVehicleDTO createVehicleDTO);
        Task<ReturnVehicleDTO?> UpdateVehicleDetailsAsync(int vehicleId, UpdateVehicleDTO updateVehicleDTO);
        Task<bool> DeleteVehicleAsync(int vehicleId);
        Task<ReturnVehicleDTO?> GetVehicleByIdAsync(int vehicleId);

        // --- CHANGE IS HERE ---
        Task<PagedResultDTO<ReturnVehicleDTO>> SearchVehiclesAsync(VehicleSearchCriteriaDTO criteria);
    }
}