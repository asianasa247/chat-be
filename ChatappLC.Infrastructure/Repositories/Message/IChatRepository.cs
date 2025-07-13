namespace ChatappLC.Infrastructure.Repositories.Message;

internal interface IChatRepository
{
    Task<ChatMessage> CreateChatMessageAsync(ChatMessage message);
    Task<ChatMessage> GetChatMessageByIdAsync(string id);
    Task<IEnumerable<ChatMessage>> GetChatMessagesByRoomIdAsync(string chatRoomId);
    Task UpdateMessageAsync(ChatMessage message);
    Task DeleteMessageAsync(string id);
}
