namespace RoadReadyAPI.DTOs
{
    public class ReturnLocationDTO
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}