using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Data.Seeders
{
    public static class LocationSeeder
    {
        public static async Task SeedAsync(RoadReadyContext context, ILogger logger)
        {
            if (!await context.Locations.AnyAsync())
            {
                logger.LogInformation("Seeding locations...");

                var locations = new List<Location>
                {
                    new Location { StoreName = "Chennai Airport (MAA)", Address = "GST Road, Meenambakkam", City = "Chennai", State = "Tamil Nadu" },
                    new Location { StoreName = "Bangalore Downtown", Address = "MG Road, Shanthala Nagar", City = "Bengaluru", State = "Karnataka" },
                    new Location { StoreName = "Mumbai Central Station", Address = "Dr Anandarao Nair Marg", City = "Mumbai", State = "Maharashtra" }
                };

                await context.Locations.AddRangeAsync(locations);
                await context.SaveChangesAsync();
                logger.LogInformation("Locations seeded successfully.");
            }
        }
    }
}
