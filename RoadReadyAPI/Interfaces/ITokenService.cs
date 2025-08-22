using RoadReadyAPI.Models;

namespace RoadReadyAPI.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
