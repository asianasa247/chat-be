namespace ManageEmployee.Services.Interfaces
{
    public interface ITelegramBotService
    {
        Task SendMessageAsync(string chatId, string message);
    }
}
