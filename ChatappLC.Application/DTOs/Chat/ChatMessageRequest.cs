namespace ChatappLC.Application.DTOs.Chat;

public class ChatMessageRequest
{
    public string SenderId { get; set; } = string.Empty;
    public string ChatRoomId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = "sent";
    public string MessageType { get; set; } = string.Empty;
    public string AttachmentUrl { get; set; } = string.Empty;
    public long FileSize { get; set; } = 0;
}
