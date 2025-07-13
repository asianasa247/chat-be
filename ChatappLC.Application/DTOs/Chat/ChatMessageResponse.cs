namespace ChatappLC.Application.DTOs.Chat;

public class ChatMessageResponse : ChatMessageRequest
{
    #region Attributes
    public string Id { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    #endregion

    #region Business Logic
    public static ChatMessageResponse MapToResponseDTO(ChatMessage message)
    {
        return new ChatMessageResponse
        {
            Id = message.Id.ToString(),
            SenderId = message.SenderId,
            ChatRoomId = message.ChatRoomId,
            Content = message.Content,
            Timestamp = message.Timestamp,
            Status = message.Status,
            MessageType = message.MessageType,
            AttachmentUrl = message.AttachmentUrl,
            FileSize = message.FileSize
        };
    }
    #endregion
}
