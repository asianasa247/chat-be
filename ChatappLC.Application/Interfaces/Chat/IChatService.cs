namespace ChatappLC.Application.Interfaces.Chat;

public interface IChatService
{
    Task<ResponseDTO<ChatMessageResponse>> CreateChatMessageAsync(ChatMessageRequest createDTO);
    Task<ResponseDTO<ChatMessageResponse>> RecallMessageAsync(string messageId);
    Task<ResponseDTO<ChatMessageResponse>> DeleteMessageAsync(string messageId);
    Task<ResponseDTO<PagedResult<ChatMessageResponse>>> GetMessagesByRoomIdAsync(string chatRoomId, int pageNumber, int pageSize);

}
