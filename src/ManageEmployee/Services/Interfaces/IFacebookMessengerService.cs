using ManageEmployee.DataTransferObject;

namespace ManageEmployee.Services.Interfaces
{
    public interface IFacebookMessengerService
    {
        Task SendMessageAsync(string psid, string message);
        Task SendButtonTemplateAsync(string recipientId, string message, List<MessengerButton> buttons);
    }
}
