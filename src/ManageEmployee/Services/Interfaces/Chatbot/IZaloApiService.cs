namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface IZaloApiService
    {
        Task SendTextAsync(string userId, string text, CancellationToken ct = default);
    }
}
