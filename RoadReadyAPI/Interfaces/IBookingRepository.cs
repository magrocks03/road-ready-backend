using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IBookingRepository : IRepository<int, Booking>
    {
        Task<bool> IsVehicleBookedForDateRangeAsync(int vehicleId, DateTime startDate, DateTime endDate);
        Task<Booking?> GetBookingDetailsByIdAsync(int bookingId);
        Task<List<Booking>> GetUserBookingsWithDetailsAsync(int userId);
        Task<List<Booking>> GetAllBookingsWithDetailsAsync();
        Task<List<Vehicle>> GetMostPopularVehiclesAsync(int count);
        Task<int> GetTotalBookingCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<List<Booking>> GetPagedUserBookingsAsync(int userId, PaginationDTO pagination);
        Task<int> GetUserBookingsCountAsync(int userId);

        // --- NEW METHODS ADDED HERE ---
        Task<List<Booking>> GetPagedAllBookingsAsync(PaginationDTO pagination);
        Task<int> GetTotalAllBookingsCountAsync();
    }
}