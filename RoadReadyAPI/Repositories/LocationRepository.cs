using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class LocationRepository : RepositoryDB<int, Location>, ILocationRepository
    {
        public LocationRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Location?> GetById(int key)
        {
            return await _context.Locations.SingleOrDefaultAsync(l => l.Id == key);
        }
    }
}