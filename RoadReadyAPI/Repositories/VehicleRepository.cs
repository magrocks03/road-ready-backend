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
    public class VehicleRepository : RepositoryDB<int, Vehicle>, IVehicleRepository
    {
        public VehicleRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Vehicle?> GetById(int key)
        {
            return await _context.Vehicles.SingleOrDefaultAsync(v => v.Id == key);
        }

        public async Task<Vehicle?> GetVehicleDetailsByIdAsync(int vehicleId)
        {
            return await _context.Vehicles
                                 .Include(v => v.Brand)
                                 .Include(v => v.Location)
                                 .FirstOrDefaultAsync(v => v.Id == vehicleId);
        }

        // This is the private helper method. It's job is to build the base query with all the filters.
        private IQueryable<Vehicle> BuildSearchQuery(VehicleSearchCriteriaDTO criteria)
        {
            var query = _context.Vehicles
                                .Include(v => v.Brand)
                                .Include(v => v.Location)
                                .AsQueryable();

            // Dynamically add filters
            if (criteria.LocationId.HasValue) query = query.Where(v => v.LocationId == criteria.LocationId.Value);
            if (criteria.BrandId.HasValue) query = query.Where(v => v.BrandId == criteria.BrandId.Value);
            if (!string.IsNullOrEmpty(criteria.Model)) query = query.Where(v => v.Model.Contains(criteria.Model));
            if (criteria.MinPrice.HasValue) query = query.Where(v => v.PricePerDay >= criteria.MinPrice.Value);
            if (criteria.MaxPrice.HasValue) query = query.Where(v => v.PricePerDay <= criteria.MaxPrice.Value);
            if (criteria.IsAvailable.HasValue) query = query.Where(v => v.IsAvailable == criteria.IsAvailable.Value);
            if (!string.IsNullOrEmpty(criteria.FuelType)) query = query.Where(v => v.FuelType.ToLower() == criteria.FuelType.ToLower());
            if (!string.IsNullOrEmpty(criteria.Transmission)) query = query.Where(v => v.Transmission.ToLower() == criteria.Transmission.ToLower());
            if (criteria.SeatingCapacity.HasValue) query = query.Where(v => v.SeatingCapacity >= criteria.SeatingCapacity.Value);
            if (!string.IsNullOrEmpty(criteria.BrandName))
            {
                query = query.Where(v => v.Brand.Name.Contains(criteria.BrandName));
            }
            if (!string.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(v => v.Name.Contains(criteria.Name));
            }
            return query;
        }

        // The public methods now use the private helper method.
        public async Task<List<Vehicle>> GetPagedVehiclesAsync(VehicleSearchCriteriaDTO criteria)
        {
            var query = BuildSearchQuery(criteria);
            return await query.Skip((criteria.PageNumber - 1) * criteria.PageSize)
                              .Take(criteria.PageSize)
                              .ToListAsync();
        }

        public async Task<int> GetTotalVehicleCountAsync(VehicleSearchCriteriaDTO criteria)
        {
            var query = BuildSearchQuery(criteria);
            return await query.CountAsync();
        }
    }
}