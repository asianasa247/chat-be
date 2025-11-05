namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface IZaloApiService
    {
        // NEW: gửi theo appCode
        Task SendTextAsync(string appCode, string userId, string text, CancellationToken ct = default);

        // NEW: cho job pre-warm/refresh token theo appCode
        Task EnsureAccessTokenAsync(string appCode, CancellationToken ct = default);
    }
}
