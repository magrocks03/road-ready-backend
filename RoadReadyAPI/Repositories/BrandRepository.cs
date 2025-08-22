using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class BrandRepository : RepositoryDB<int, Brand>, IBrandRepository
    {
        public BrandRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Brand?> GetById(int key)
        {
            return await _context.Brands.SingleOrDefaultAsync(b => b.Id == key);
        }
    }
}