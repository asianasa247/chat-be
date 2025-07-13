namespace ChatappLC.Application.DTOs.Chat;

public class ChatRoomRequest
{
    public string RoomName { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public List<string> ParticipantIds { get; set; } = new List<string>();
    public bool IsGroup { get; set; } = false;
}

public class ChatRoomUpdate : ChatRoomRequest
{
    public string Id { get; set; } = string.Empty;
}
