using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class BookingRepository : RepositoryDB<int, Booking>, IBookingRepository
    {
        public BookingRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Booking?> GetById(int key)
        {
            return await _context.Bookings.SingleOrDefaultAsync(b => b.Id == key);
        }

        public async Task<bool> IsVehicleBookedForDateRangeAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .AnyAsync(b => b.VehicleId == vehicleId &&
                               b.Status.Name != "Cancelled" &&
                               startDate < b.EndDate &&
                               endDate > b.StartDate);
        }

        public async Task<Booking?> GetBookingDetailsByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle).ThenInclude(v => v.Location)
                .Include(b => b.Vehicle).ThenInclude(v => v.Brand)
                .Include(b => b.Status)
                .Include(b => b.BookingExtras).ThenInclude(be => be.Extra)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<List<Booking>> GetUserBookingsWithDetailsAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Vehicle).ThenInclude(v => v.Location)
                .Include(b => b.Vehicle).ThenInclude(v => v.Brand)
                .Include(b => b.Status)
                .Include(b => b.BookingExtras).ThenInclude(be => be.Extra)
                .Include(b => b.Payment)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllBookingsWithDetailsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Vehicle).ThenInclude(v => v.Location)
                .Include(b => b.Vehicle).ThenInclude(v => v.Brand)
                .Include(b => b.Status)
                .Include(b => b.BookingExtras).ThenInclude(be => be.Extra)
                .Include(b => b.Payment)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetMostPopularVehiclesAsync(int count)
        {
            return await _context.Bookings
                .Where(b => b.Status.Name == "Completed")
                .GroupBy(b => b.Vehicle)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetTotalBookingCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Bookings
                                 .Where(b => b.Status.Name == "Completed" || b.Status.Name == "Cancelled - No Refund")
                                 .SumAsync(b => b.TotalCost);
        }

        public async Task<List<Booking>> GetPagedUserBookingsAsync(int userId, PaginationDTO pagination)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Vehicle).ThenInclude(v => v.Location)
                .Include(b => b.Vehicle).ThenInclude(v => v.Brand)
                .Include(b => b.Status)
                .Include(b => b.BookingExtras).ThenInclude(be => be.Extra)
                .Include(b => b.Payment)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserBookingsCountAsync(int userId)
        {
            return await _context.Bookings.CountAsync(b => b.UserId == userId);
        }

        public async Task<List<Booking>> GetPagedAllBookingsAsync(PaginationDTO pagination)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle).ThenInclude(v => v.Location)
                .Include(b => b.Vehicle).ThenInclude(v => v.Brand)
                .Include(b => b.Status)
                .Include(b => b.BookingExtras).ThenInclude(be => be.Extra)
                .Include(b => b.Payment)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalAllBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }
    }
}