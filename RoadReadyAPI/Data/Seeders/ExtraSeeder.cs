using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Data.Seeders
{
    public static class ExtraSeeder
    {
        public static async Task SeedAsync(RoadReadyContext context, ILogger logger)
        {
            if (!await context.Extras.AnyAsync())
            {
                logger.LogInformation("Seeding optional extras...");

                var extras = new List<Extra>
                {
                    new Extra { Name = "GPS Navigation System", Price = 500, PriceType = PriceType.FlatFee },
                    new Extra { Name = "Child Safety Seat", Price = 250, PriceType = PriceType.PerDay },
                    new Extra { Name = "Booster Seat", Price = 200, PriceType = PriceType.PerDay },
                    new Extra { Name = "Additional Driver", Price = 1000, PriceType = PriceType.FlatFee },
                    new Extra { Name = "Collision Damage Waiver", Price = 800, PriceType = PriceType.PerDay },
                    new Extra { Name = "Prepaid Fuel Option", Price = 3000, PriceType = PriceType.FlatFee }
                };

                await context.Extras.AddRangeAsync(extras);
                await context.SaveChangesAsync();
                logger.LogInformation("Optional extras seeded successfully.");
            }
        }
    }
}