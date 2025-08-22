using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Data.Seeders;
using System;
using System.Threading.Tasks;

namespace RoadReadyAPI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            // Get the necessary services
            var context = serviceProvider.GetRequiredService<RoadReadyContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // Ensure the database is created and migrations are applied
            await context.Database.MigrateAsync();

            // Call each seeder in the correct order.
            await RoleSeeder.SeedAsync(context, logger);
            await AdminUserSeeder.SeedAsync(context, logger);
            await BrandSeeder.SeedAsync(context, logger);
            await BookingStatusSeeder.SeedAsync(context, logger);
            await LocationSeeder.SeedAsync(context, logger);

            // --- NEW SEEDER CALLED HERE ---
            await ExtraSeeder.SeedAsync(context, logger);
        }
    }
}