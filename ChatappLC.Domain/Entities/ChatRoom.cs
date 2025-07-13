namespace ChatappLC.Domain.Entities;

public class ChatRoom
{
    #region Attributes
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; private set; }

    [BsonElement("roomName")]
    public string RoomName { get; private set; } = string.Empty;

    [BsonElement("participantIds")]
    public List<string> ParticipantIds { get; private set; } = new List<string>();

    [BsonElement("lastMessage")]
    public string LastMessage { get; private set; } = string.Empty;

    [BsonElement("lastMessageTime")]
    public DateTime? LastMessageTime { get; private set; }

    [BsonElement("creator_id")]
    public string CreatorId { get; private set; } = string.Empty;

    [BsonElement("isGroup")]
    public bool IsGroup { get; private set; } = false;
    [BsonElement("imageGroup")]
    public string ImageGroup { get; private set; } = string.Empty;

    #endregion

    #region Business Logic
    public static ChatRoom Create(string RoomName, List<string> ParticipantIds, string LastMessage, string creatorId, bool isGroup = false, string image = "")
    {
        return new ChatRoom
        {
            Id = ObjectId.GenerateNewId(),
            RoomName = RoomName,
            ParticipantIds = ParticipantIds,
            LastMessage = LastMessage,
            LastMessageTime = TimeZoneHelper.GetVietNamTimeNow(),
            CreatorId = creatorId,
            IsGroup = isGroup,
            ImageGroup = image,
        };
    }
    //thêm thành viên
    public void AddParticipant(string participantId)
    {
        if (!ParticipantIds.Contains(participantId))
        {
            ParticipantIds.Add(participantId);
        }
    }

    public void UpdateLastMessage(string lastMessage)
    {
        LastMessage = lastMessage;
        LastMessageTime = TimeZoneHelper.GetVietNamTimeNow();
    }
    #endregion

}
