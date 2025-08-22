using RoadReadyAPI.DTOs;
using System.Threading.Tasks;

namespace RoadReadyAPI.Interfaces
{
    public interface IUserService
    {
        Task<ReturnUserDTO> Login(LoginUserDTO loginUserDTO);
        Task<ReturnUserDTO> Register(RegisterUserDTO registerUserDTO);
    }
}