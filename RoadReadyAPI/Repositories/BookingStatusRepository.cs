using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class BookingStatusRepository : RepositoryDB<int, BookingStatus>, IBookingStatusRepository
    {
        public BookingStatusRepository(RoadReadyContext context) : base(context) { }

        public override async Task<BookingStatus?> GetById(int key)
        {
            return await _context.BookingStatuses.SingleOrDefaultAsync(bs => bs.Id == key);
        }

        // --- IMPLEMENTATION OF NEW METHOD ---
        public async Task<BookingStatus?> GetStatusByNameAsync(string statusName)
        {
            return await _context.BookingStatuses.FirstOrDefaultAsync(s => s.Name == statusName);
        }
    }
}