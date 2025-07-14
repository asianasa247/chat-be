using ChatappLC.Application.DTOs.Auth;

namespace ChatappLC.Application.Interfaces.User
{
    public interface IUserService
    {
        Task<ResponseDTO<UserResponse>> RegisterAsync(UserRegisterRequest request);
        Task<List<UserResponse>> GetAllUsersAsync();
        Task<ResponseDTO<TokenResponse>> LoginWithTokenAsync(UserLoginRequest request);
        Task<ResponseDTO<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> ForgotPasswordAsync(UserForgotPasswordRequest request);
        Task<List<UserResponse>> GetAllUsersExceptAsync(string currentUserId);
        Task<List<UserInfoDTO>> GetUsersByListIdsAsync(List<string> userIds);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> VerifyEmailAsync(string token);

    }
}