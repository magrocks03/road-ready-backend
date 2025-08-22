using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class ExtraRepository : RepositoryDB<int, Extra>, IExtraRepository
    {
        public ExtraRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Extra?> GetById(int key)
        {
            return await _context.Extras.SingleOrDefaultAsync(e => e.Id == key);
        }

        // --- IMPLEMENTATION OF NEW METHOD ---
        public async Task<List<Extra>> GetExtrasByIdsAsync(List<int> extraIds)

        {
            return await _context.Extras.Where(e => extraIds.Contains(e.Id)).ToListAsync();
        }
    }
}