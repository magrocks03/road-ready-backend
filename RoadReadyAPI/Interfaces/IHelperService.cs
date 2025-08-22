using RoadReadyAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IHelperService
    {
        /// <summary>
        /// Gets a list of all car brands.
        /// </summary>
        /// <returns>A list of car brands.</returns>
        Task<List<ReturnBrandDTO>> GetAllBrandsAsync();

        /// <summary>
        /// Gets a list of all rental locations.
        /// </summary>
        /// <returns>A list of rental locations.</returns>
        Task<List<ReturnLocationDTO>> GetAllLocationsAsync();

        /// <summary>
        /// Gets a list of all available optional extras.
        /// </summary>
        /// <returns>A list of optional extras.</returns>
        Task<List<ReturnExtraDTO>> GetAllExtrasAsync();
    }
}