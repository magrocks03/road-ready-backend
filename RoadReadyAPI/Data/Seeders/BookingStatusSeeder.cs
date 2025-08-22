using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Data.Seeders
{
    public static class BookingStatusSeeder
    {
        public static async Task SeedAsync(RoadReadyContext context, ILogger logger)
        {
            if (!await context.BookingStatuses.AnyAsync())
            {
                logger.LogInformation("Seeding booking statuses...");

                // --- FIX IS HERE ---
                // We are adding all the required statuses for our business logic.
                var statuses = new[]
                {
                    "Pending",
                    "Confirmed",
                    "Completed",
                    "Cancelled",
                    "Cancelled - Refund Pending",
                    "Cancelled - No Refund",
                    "Cancelled - Refunded"
                };

                foreach (var statusName in statuses)
                {
                    await context.BookingStatuses.AddAsync(new BookingStatus { Name = statusName });
                }
                await context.SaveChangesAsync();
                logger.LogInformation("Booking statuses seeded successfully.");
            }
        }
    }
}
