using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Data.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoadReadyContext context, ILogger logger)
        {
            if (!await context.Roles.AnyAsync())
            {
                logger.LogInformation("Seeding roles...");
                var roles = new[] { "Admin", "Customer", "Rental Agent" };
                foreach (var roleName in roles)
                {
                    await context.Roles.AddAsync(new Role { Name = roleName });
                }
                await context.SaveChangesAsync();
                logger.LogInformation("Roles seeded successfully.");
            }
        }
    }
}