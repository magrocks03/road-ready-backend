using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RoadReadyAPI.Data.Seeders
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAsync(RoadReadyContext context, ILogger logger)
        {
            if (!await context.Users.AnyAsync(u => u.Email == "admin@roadready.com"))
            {
                logger.LogInformation("Seeding default admin user...");
                using var hmac = new HMACSHA512();
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@roadready.com",
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Admin@123")),
                    PasswordHashKey = hmac.Key
                };
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    await context.UserRoles.AddAsync(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                    await context.SaveChangesAsync();
                    logger.LogInformation("Default admin user created and assigned Admin role.");
                }
                else
                {
                    logger.LogWarning("Admin role not found. Could not assign role to default admin user.");
                }
            }
        }
    }
}