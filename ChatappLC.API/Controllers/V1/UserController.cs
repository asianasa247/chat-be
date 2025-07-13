namespace ChatappLC.API.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        return HandleResponse(result);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        var result = await _userService.LoginWithTokenAsync(request);
        return HandleResponse(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var result = await _userService.RefreshTokenAsync(request);
        return HandleResponse(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(UserForgotPasswordRequest request)
    {
        var success = await _userService.ForgotPasswordAsync(request);
        if (!success)
            return NotFound(new { message = "Email not found." });

        return Ok(new { message = "Reset instructions sent (mock)." });
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsersAsync()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var users = await _userService.GetAllUsersExceptAsync(currentUserId);
        return Ok(users);
    }
}
