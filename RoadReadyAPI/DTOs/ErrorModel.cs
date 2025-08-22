namespace RoadReadyAPI.DTOs
{
    /// <summary>
    /// A standard model for returning API error responses.
    /// </summary>
    public class ErrorModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ErrorModel(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}