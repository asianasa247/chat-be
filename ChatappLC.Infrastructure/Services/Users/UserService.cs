namespace ChatappLC.Infrastructure.Services.Users;

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
        var checkMailFakeOrReal = await _emailService.VerifyEmailWithMailboxValidator(request.Email);
        if (!checkMailFakeOrReal)
            return new ResponseDTO<UserResponse>(false, "The email you entered does not exist. Please register with your real email address!", null);

        var user = new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Email = request.Email,
            Username = request.Username,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password // ⚠️ Bạn nên mã hóa password trong môi trường thật
        };

        await _context.Users.InsertOneAsync(user);

        return new ResponseDTO<UserResponse>(true, "Registration successful", new UserResponse
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
}