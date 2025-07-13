namespace ChatappLC.Domain.Entities;
public class ChatMessage
{
    #region Attributes
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; private set; }

    [BsonElement("senderId")]
    public string SenderId { get; private set; } = string.Empty;

    [BsonElement("chatRoomId")]
    public string ChatRoomId { get; private set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; private set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; private set; } = TimeZoneHelper.GetVietNamTimeNow();

    [BsonElement("status")]
    public string Status { get; private set; } = "sent";

    [BsonElement("messageType")]
    public string MessageType { get; private set; } = string.Empty;

    [BsonElement("attachmentUrl")]
    public string AttachmentUrl { get; private set; } = string.Empty;

    [BsonElement("fileSize")]
    public long FileSize { get; private set; } = 0;
    #endregion

    #region Business Logic
    public static ChatMessage Create(string SenderId, string ChatRoomId, string Content, string MessageType, string AttachmentUrl, long FileSize)
    {
        return new ChatMessage
        {
            Id = ObjectId.GenerateNewId(),
            SenderId = SenderId,
            ChatRoomId = ChatRoomId,
            Content = Content,
            MessageType = MessageType,
            AttachmentUrl = AttachmentUrl,
            FileSize = FileSize,
            Timestamp = TimeZoneHelper.GetVietNamTimeNow(),
        };
    }

    public void UpdateStatus(string status)
    {
        Status = status;
    }
    #endregion

}
