namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface IZaloChatbotService
    {
        Task<string> BuildReplyAsync(string userText, CancellationToken ct = default);
    }
}
