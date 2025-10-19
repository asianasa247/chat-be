namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface IZaloChatbotService
    {
        // Lưu ý: thêm userId để quản lý state theo người dùng
        Task<string> BuildReplyAsync(string userId, string userText, CancellationToken ct = default);
    }
}
