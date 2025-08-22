using System;

namespace RoadReadyAPI.DTOs
{
    public class ReturnPaymentDTO
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
    }
}