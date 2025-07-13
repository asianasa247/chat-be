using ChatappLC.Application.DTOs.Auth;

namespace ChatappLC.Application.Interfaces.Auth
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserResponse user);
        TokenResponse GenerateTokens(UserResponse user);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        string GenerateRefreshToken();
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}