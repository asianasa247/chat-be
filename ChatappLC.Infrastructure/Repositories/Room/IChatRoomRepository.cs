namespace ChatappLC.Infrastructure.Repositories.Room;

internal interface IChatRoomRepository
{
    Task<ChatRoom> CreateChatRoomAsync(ChatRoom chatRoom);
    Task<ChatRoom> GetChatRoomByIdAsync(string id);
    Task<IEnumerable<ChatRoom>> GetChatRoomsByUserIdAsync(string userId);
    Task<bool> UpdateChatRoomAsync(ChatRoom chatRoom);
    Task<ChatRoom> GetChatRoomByUserOneAndOne(string userOne, string userTwo);
}
