using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Data.Seeders
{
    public static class BrandSeeder
    {
        public static async Task SeedAsync(RoadReadyContext context, ILogger logger)
        {
            if (!await context.Brands.AnyAsync())
            {
                logger.LogInformation("Seeding brands...");
                var brands = new[] { "Toyota", "Ford", "BMW", "Honda", "Nissan", "Mercedes-Benz", "Audi" };
                foreach (var brandName in brands)
                {
                    await context.Brands.AddAsync(new Brand { Name = brandName });
                }
                await context.SaveChangesAsync();
                logger.LogInformation("Brands seeded successfully.");
            }
        }
    }
}
