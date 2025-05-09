using CarShare.BLL.DTOs.User;
using System.Threading.Tasks;

namespace CarShare.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterAsync(UserCreateDTO userDTO);
        Task<string> LoginAsync(UserLoginDTO loginDTO);
        Task<UserResponseDTO> GetProfileAsync(Guid userId);
        Task VerifyCarOwnerAsync(Guid ownerId);
        // Add these for admin approval
        Task<IEnumerable<UserResponseDTO>> GetPendingUsersAsync();
        Task ApproveUserAsync(Guid userId);
        Task RejectUserAsync(Guid userId, string? reason = null);  // Optional rejection reason
    }
}