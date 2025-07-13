namespace ChatappLC.Application.Interfaces.Chat;

public interface IChatRoomService
{
    Task<ResponseDTO<ChatRoomResponse>> CreateChatRoomAsync(ChatRoomRequest createDTO);
    Task<ResponseDTO<ChatRoomResponse>> GetChatRoomByIdAsync(string id);
    Task<ResponseDTO<IEnumerable<ChatRoomResponse>>> GetChatRoomsByUserIdAsync();
    Task<bool> UpdateChatRoomAsync(ChatRoomUpdate chatRoomDTO);
    Task<ResponseDTO<ChatRoomResponse>> ShowRoomOrCreateRoomForOneAndOneAsync(ChatRoomRequest createDTO);
    Task<bool> GetUserInRoomAsync(string userId, string roomId);

}
