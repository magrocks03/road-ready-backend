using Microsoft.EntityFrameworkCore;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System.Threading.Tasks;

namespace RoadReadyAPI.Repositories
{
    public class PaymentRepository : RepositoryDB<int, Payment>, IPaymentRepository
    {
        public PaymentRepository(RoadReadyContext context) : base(context) { }

        public override async Task<Payment?> GetById(int key)
        {
            return await _context.Payments.SingleOrDefaultAsync(p => p.Id == key);
        }
    }
}
