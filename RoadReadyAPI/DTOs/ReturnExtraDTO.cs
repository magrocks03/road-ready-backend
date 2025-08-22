using RoadReadyAPI.Models;

namespace RoadReadyAPI.DTOs
{
    public class ReturnExtraDTO
    {
        public int ExtraId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public PriceType PriceType { get; set; }
    }
}