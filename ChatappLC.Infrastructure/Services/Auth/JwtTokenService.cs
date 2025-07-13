using ChatappLC.Application.DTOs.User;
using ChatappLC.Application.DTOs.Auth;
using ChatappLC.Application.Interfaces.Auth;
using ChatappLC.Domain.Entities;
using ChatappLC.Infrastructure.MongoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatappLC.Infrastructure.Services.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly MongoDbContext _context;

        public JwtTokenService(IConfiguration config, MongoDbContext context)
        {
            _config = config;
            _context = context;
        }

        public string GenerateToken(UserResponse user)
        {
            var secretKey = _config["JwtSettings:SecretKey"]
                ?? throw new ArgumentNullException("JwtSettings:SecretKey is missing in configuration");

            var issuer = _config["JwtSettings:Issuer"]
                ?? throw new ArgumentNullException("JwtSettings:Issuer is missing in configuration");

            var audience = _config["JwtSettings:Audience"]
                ?? throw new ArgumentNullException("JwtSettings:Audience is missing in configuration");

            var expiresInMinutesStr = _config["JwtSettings:ExpiresInMinutes"]
                ?? throw new ArgumentNullException("JwtSettings:ExpiresInMinutes is missing in configuration");

            if (!double.TryParse(expiresInMinutesStr, out var expiresInMinutes))
                throw new FormatException("JwtSettings:ExpiresInMinutes must be a number");

            var key = Encoding.UTF8.GetBytes(secretKey);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.Id),
                new Claim("Email", user.Email),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public TokenResponse GenerateTokens(UserResponse user)
        {
            var accessToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpires = DateTime.UtcNow.AddDays(30); // Refresh token valid for 30 days

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = refreshTokenExpires
            };

            _context.RefreshTokens.InsertOneAsync(refreshTokenEntity);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Find(rt => rt.Token == refreshToken && rt.IsRevoked == false)
                .FirstOrDefaultAsync();

            if (storedToken == null || !storedToken.IsActive)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            // Get user
            var user = await _context.Users
                .Find(u => u.Id == storedToken.UserId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new SecurityTokenException("User not found");
            }

            // Revoke old refresh token
            await RevokeRefreshTokenAsync(refreshToken);

            // Generate new tokens
            var userResponse = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber
            };

            return GenerateTokens(userResponse);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Find(rt => rt.Token == refreshToken)
                .FirstOrDefaultAsync();

            return storedToken != null && storedToken.IsActive;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var filter = Builders<RefreshToken>.Filter.Eq(rt => rt.Token, refreshToken);
            var update = Builders<RefreshToken>.Update
                .Set(rt => rt.IsRevoked, true)
                .Set(rt => rt.RevokedAt, DateTime.UtcNow);

            await _context.RefreshTokens.UpdateOneAsync(filter, update);
        }
    }
}