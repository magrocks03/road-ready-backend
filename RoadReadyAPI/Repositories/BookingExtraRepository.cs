using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class BookingExtraRepository : RepositoryDB<int, BookingExtra>, IBookingExtraRepository
    {
        public BookingExtraRepository(RoadReadyContext context) : base(context) { }

        public override async Task<BookingExtra?> GetById(int key)
        {
            // Note: The primary key for BookingExtra is composite (BookingId, ExtraId).
            // This implementation of GetById will find the first entry for a given BookingId.
            // More specific query methods may be needed in the future.
            return await _context.BookingExtras.FirstOrDefaultAsync(be => be.BookingId == key);
        }
    }
}