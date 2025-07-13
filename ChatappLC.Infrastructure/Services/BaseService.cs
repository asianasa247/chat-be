namespace ChatappLC.Infrastructure.Services;

internal class BaseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected string? GetCurrentUserEmail() => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == "Email")?.Value;

    protected string? GetCurrentUserId() => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == "UserId")?.Value;

    protected string? GetCurrentRoles() => _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == "Role")?.Value;

    protected DateTime GetVietNamTimeNow() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

}
