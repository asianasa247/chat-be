namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface IGeminiNlpService
    {
        Task<string?> GenerateAsync(string prompt, CancellationToken ct = default);
    }
}
