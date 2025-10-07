namespace ManageEmployee.Services.Interfaces
{
    public interface IMessengerServices
    {
        Task SendMessageAsync(string messageId, string messageText);
        Task SendButtonTemplateAsync(string recipientId, string text, List<(string title, string payload)> buttons);

    }
}
