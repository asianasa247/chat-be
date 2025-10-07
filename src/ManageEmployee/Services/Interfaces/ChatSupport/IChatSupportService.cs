using ManageEmployee.Entities.ChatSupport;

namespace ManageEmployee.Services.Interfaces.ChatSupport;

public interface IChatSupportService
{
    Task<ChatSupportRoom> CreateRoomAsync(string name, string kind, IEnumerable<string> participantIds, string ownerId, string? ownerName);
    Task<IEnumerable<ChatSupportRoom>> GetMyRoomsAsync(string userId);

    Task<ChatSupportMessage> AddMessageAsync(int roomId, string senderId, string? senderName, string content, string? attachmentUrl, string? contentType);
    Task<IReadOnlyList<ChatSupportMessage>> GetMessagesAsync(int roomId, int page = 1, int pageSize = 50);
    Task<bool> IsParticipantAsync(int roomId, string userId);
}
