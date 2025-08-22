using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Contexts
{
    public class RoadReadyContext : DbContext
    {
        public RoadReadyContext(DbContextOptions options) : base(options) { }

        // DbSets for all the models
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<BookingStatus> BookingStatuses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Location> Locations { get; set; }

        // --- NEW DbSets ADDED HERE ---
        public DbSet<Extra> Extras { get; set; }
        public DbSet<BookingExtra> BookingExtras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Existing Configurations ---
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>().HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>().HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Vehicle>().Property(v => v.PricePerDay).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Booking>().Property(b => b.TotalCost).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Refund>().Property(r => r.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Extra>().Property(e => e.Price).HasColumnType("decimal(18,2)"); // Also configure Extra price

            modelBuilder.Entity<Review>().HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Review>().HasOne(r => r.Vehicle).WithMany().HasForeignKey(r => r.VehicleId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Refund>().HasOne(r => r.AdminUser).WithMany().HasForeignKey(r => r.AdminUserId).OnDelete(DeleteBehavior.Restrict);

            // ================================================================
            //                        *** NEW CODE ADDED HERE ***
            // ================================================================
            // Configure the composite primary key for the BookingExtras junction table
            modelBuilder.Entity<BookingExtra>().HasKey(be => new { be.BookingId, be.ExtraId });

            // Configure the many-to-many relationship between Bookings and Extras
            modelBuilder.Entity<BookingExtra>()
                .HasOne(be => be.Booking)
                .WithMany(b => b.BookingExtras)
                .HasForeignKey(be => be.BookingId);

            modelBuilder.Entity<BookingExtra>()
                .HasOne(be => be.Extra)
                .WithMany(e => e.BookingExtras)
                .HasForeignKey(be => be.ExtraId);
        }
    }
}