using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class RefundRepository : RepositoryDB<int, Refund>, IRefundRepository
    {
        public RefundRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Refund?> GetById(int key)
        {
            return await _context.Refunds.SingleOrDefaultAsync(r => r.Id == key);
        }

        // --- IMPLEMENTATION OF NEW METHOD ---
        public async Task<decimal> GetTotalRefundsAmountAsync()
        {
            // This efficiently calculates the sum of the 'Amount' column for all refunds.
            return await _context.Refunds.SumAsync(r => r.Amount);
        }
    }
}