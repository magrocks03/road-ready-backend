using RoadReadyAPI.Models;

namespace RoadReadyAPI.Interfaces
{
    public interface IRefundRepository : IRepository<int, Refund> 
    {
        Task<decimal> GetTotalRefundsAmountAsync();
    }
}
