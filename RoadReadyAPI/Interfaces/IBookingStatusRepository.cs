using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IBookingStatusRepository : IRepository<int, BookingStatus>
    {
        // --- NEW METHOD ADDED HERE ---
        Task<BookingStatus?> GetStatusByNameAsync(string statusName);
    }
}