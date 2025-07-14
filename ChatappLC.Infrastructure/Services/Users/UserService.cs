namespace ChatappLC.Infrastructure.Services.Users;
using BCrypt.Net;
using MongoDB.Driver;
using ChatappLC.Application.DTOs;
using ChatappLC.Application.Interfaces;
using ChatappLC.Domain.Entities;

using MongoDB.Bson;

public class UserService : IUserService
{
    private readonly MongoDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenService _jwtTokenService;

    public UserService(MongoDbContext context, IJwtTokenService jwtTokenService, IEmailService emailService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
    }

    public async Task<ResponseDTO<UserResponse>> RegisterAsync(UserRegisterRequest request)
    {
        // 1. Kiểm tra email có tồn tại (MailboxValidator hoặc API bên ngoài)
        var checkMailFakeOrReal = await _emailService.VerifyEmailWithMailboxValidator(request.Email);
        if (!checkMailFakeOrReal)
        {
            return new ResponseDTO<UserResponse>(false, "The email you entered does not exist. Please register with your real email address!", null);
        }

        // 2. Kiểm tra trùng email
        var existingUser = await _context.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            return new ResponseDTO<UserResponse>(false, "Email already exists. Please use a different email.", null);
        }

        var hashedPassword = BCrypt.HashPassword(request.Password);

        // 4. Tạo user mới
        var verifyToken = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Email = request.Email,
            Username = request.Username,
            PhoneNumber = request.PhoneNumber,
            Password = hashedPassword,
            IsVerify = false,
            VerifyToken = verifyToken
        };

        await _context.Users.InsertOneAsync(user);

        // 5. Tạo link xác thực
        string verifyUrl = $"https://localhost:7139/api/user/verify-email?token={verifyToken}";

        // 6. Gửi email xác thực
        string subject = "Verify Your Account";

        string body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; color: #333; background-color: #f9f9f9;'>
                <h2 style='color: #2c3e50;'>Hello {user.Username},</h2>
                <p>Thank you for signing up! Please confirm your email address by clicking the button below:</p>
    
                <div style='margin: 30px 0;'>
                    <a href='{verifyUrl}' style='
                        display: inline-block;
                        background-color: #3498db;
                        color: white;
                        padding: 12px 24px;
                        border-radius: 6px;
                        text-decoration: none;
                        font-weight: bold;
                    '>Verify Email</a>
                </div>

                <p>If you didn't create this account, you can safely ignore this email.</p>
                <hr style='margin: 40px 0;' />
                <p style='font-size: 12px; color: #888;'>This is an automated message. Please do not reply.</p>
            </div>";
        await _emailService.SendEmailAsync(user.Email, subject, body);

        // 7. Trả về kết quả
        return new ResponseDTO<UserResponse>(true, "Registration successful. Please check your email to verify your account.", new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            PhoneNumber = user.PhoneNumber
        });
    }

    public async Task<List<UserResponse>> GetAllUsersAsync()
    {
        var users = await _context.Users.Find(_ => true).ToListAsync();
        return users.Select(u => new UserResponse
        {
            Id = u.Id,
            Email = u.Email,
            Username = u.Username,
            PhoneNumber = u.PhoneNumber
        }).ToList();
    }

    public async Task<List<UserInfoDTO>> GetUsersByListIdsAsync(List<string> userIds)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.In(u => u.Id, userIds)
        );

        var users = await _context.Users.Find(filter).ToListAsync();

        var userLists = users.Select(user => new UserInfoDTO
        {
            UserId = user.Id,
            FullName = user.FullName,
            Image = !string.IsNullOrEmpty(user.Image) ? user.Image : DefaultImage.DefaultAvatar,
        }).ToList();

        return userLists;
    }

    public async Task<ResponseDTO<TokenResponse>> LoginWithTokenAsync(UserLoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .Find(u => u.Email == request.Email && u.Password == request.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return new ResponseDTO<TokenResponse>(false, "Invalid email or password");

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };

            var tokenResponse = _jwtTokenService.GenerateTokens(userResponse);
            return new ResponseDTO<TokenResponse>(true, "Login successful", tokenResponse);
        }
        catch (Exception ex)
        {
            return new ResponseDTO<TokenResponse>(false, $"Login failed: {ex.Message}");
        }
    }

    public async Task<ResponseDTO<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return new ResponseDTO<TokenResponse>(false, "Refresh token is required");

            var tokenResponse = await _jwtTokenService.RefreshTokenAsync(request.RefreshToken);
            return new ResponseDTO<TokenResponse>(true, "Refresh token successful", tokenResponse);
        }
        catch (Exception ex)
        {
            return new ResponseDTO<TokenResponse>(false, $"Refresh token failed: {ex.Message}");
        }
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        try
        {
            await _jwtTokenService.RevokeRefreshTokenAsync(refreshToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ForgotPasswordAsync(UserForgotPasswordRequest request)
    {
        var user = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        return user != null; // Có thể tích hợp gửi email
    }

    public async Task<List<UserResponse>> GetAllUsersExceptAsync(string currentUserId)
    {
        var users = await _context.Users.Find(_ => true).ToListAsync();

        var filtered = users
            .Where(u => u.Id != currentUserId)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            })
            .ToList();

        return filtered;
    }
    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _context.Users.Find(u => u.VerifyToken == token).FirstOrDefaultAsync();
        if (user == null) return false;

        user.IsVerify = true;
        user.VerifyToken = null;

        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
        return true;
    }

}
