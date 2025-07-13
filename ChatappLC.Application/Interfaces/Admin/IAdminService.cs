using ChatappLC.Application.DTOs.Admin;
namespace ChatappLC.Application.Interfaces.Admin
{
    public interface IAdminService
    {
        Task<List<UserResponse>> GetAllUsersAsync();
        Task<UserResponse?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(UserUpdateRequest request);
        Task<bool> DeleteUserAsync(string id);
    }

}
