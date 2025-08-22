using RoadReadyAPI.Models;

namespace RoadReadyAPI.Interfaces
{
    // The key for BookingExtra is composite, but we use int for the generic interface.
    public interface IBookingExtraRepository : IRepository<int, BookingExtra>
    {
    }
}