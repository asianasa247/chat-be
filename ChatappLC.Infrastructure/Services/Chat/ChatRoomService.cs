namespace ChatappLC.Infrastructure.Services.Chat;

internal class ChatRoomService : BaseService, IChatRoomService
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IUserService _userService;

    public ChatRoomService(IChatRoomRepository chatRoomRepository, IUserService userService,
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _chatRoomRepository = chatRoomRepository;
        _userService = userService;
    }

    public async Task<ResponseDTO<ChatRoomResponse>> CreateChatRoomAsync(ChatRoomRequest createDTO)
    {
        if (createDTO.ParticipantIds.Count < 2)
        {
            return new ResponseDTO<ChatRoomResponse>(false, "participantIds must larger 2", null);
        }
        createDTO.ParticipantIds.Sort();
        var creatorId = createDTO.ParticipantIds.Count > 2 ? GetCurrentUserId()! : string.Empty;
        var chatRoom = ChatRoom.Create(createDTO.RoomName, createDTO.ParticipantIds, "newly created chat room", creatorId);
        var createdRoom = await _chatRoomRepository.CreateChatRoomAsync(chatRoom);
        if (createdRoom == null)
        {
            return new ResponseDTO<ChatRoomResponse>(false, "create chatRoom failed", null);
        }
        return new ResponseDTO<ChatRoomResponse>(true, "create chatRoom successfully", await MapToResponseDTO(createdRoom));
    }

    public async Task<ResponseDTO<ChatRoomResponse>> GetChatRoomByIdAsync(string id)
    {
        var chatRoom = await _chatRoomRepository.GetChatRoomByIdAsync(id);
        if (chatRoom == null)
        {
            return new ResponseDTO<ChatRoomResponse>(false, "chatRoom not found", null);
        }
        var roomResponse = await MapToResponseDTO(chatRoom);
        return new ResponseDTO<ChatRoomResponse>(true, "get chatRoom successfully", roomResponse);
    }

    public async Task<ResponseDTO<IEnumerable<ChatRoomResponse>>> GetChatRoomsByUserIdAsync()
    {
        var rooms = await _chatRoomRepository.GetChatRoomsByUserIdAsync(GetCurrentUserId()!);
        var roomDTOs = new List<ChatRoomResponse>();
        foreach (var room in rooms)
        {
            roomDTOs.Add(await MapToResponseDTO(room));
        }
        return new ResponseDTO<IEnumerable<ChatRoomResponse>>(true, "get chatRooms successfully", roomDTOs);
    }

    public async Task<bool> UpdateChatRoomAsync(ChatRoomUpdate chatRoomDTO)
    {
        var chatRoom = await _chatRoomRepository.GetChatRoomByIdAsync(chatRoomDTO.Id);
        if (chatRoom == null)
        {
            return false;
        }
        chatRoom.UpdateLastMessage(chatRoomDTO.LastMessage);
        var room = await MapToResponseDTO(chatRoom);
        await _chatRoomRepository.UpdateChatRoomAsync(chatRoom);
        return true;
    }

    public async Task<bool> GetUserInRoomAsync(string roomId, string senderId)
    {
        var chatRoom = await GetChatRoomByIdAsync(roomId);
        if (chatRoom.Data == null)
        {
            return false;
        }
        var isUserInRoom = chatRoom.Data.Participants.Any(p => p.UserId == senderId);
        return isUserInRoom;
    }

    private async Task<ChatRoomResponse> MapToResponseDTO(ChatRoom chatRoom)
    {
        Console.WriteLine("participantIds: " + chatRoom.ParticipantIds.Count());
        var participants = await _userService.GetUsersByListIdsAsync(chatRoom.ParticipantIds);
        Console.WriteLine("participants: " + participants.Count());
        return new ChatRoomResponse
        {
            Id = chatRoom.Id.ToString(),
            RoomName = chatRoom.RoomName,
            Participants = participants,
            LastMessage = chatRoom.LastMessage,
            LastMessageTime = chatRoom.LastMessageTime,
            CreatorId = chatRoom.CreatorId,
            IsGroup = chatRoom.IsGroup,
            ImageGroup = chatRoom.ImageGroup,
        };
    }

    public async Task<ResponseDTO<ChatRoomResponse>> ShowRoomOrCreateRoomForOneAndOneAsync(ChatRoomRequest createDTO)
    {
        if (createDTO.ParticipantIds.Count != 2)
        {
            return new ResponseDTO<ChatRoomResponse>(false, "participantIds must be 2", null);
        }
        createDTO.ParticipantIds.Sort();

        var chatRoom = (await GetChatRoomByUserOneAndOne(createDTO.ParticipantIds[0], createDTO.ParticipantIds[1])).Data;
        if (chatRoom == null)
        {
            var createdRoom = await CreateChatRoomAsync(createDTO);
            if (!createdRoom.Flag)
            {
                return new ResponseDTO<ChatRoomResponse>(false, createdRoom.Message, null);
            }
            chatRoom = createdRoom.Data;
        }
        if (chatRoom == null)
        {
            return new ResponseDTO<ChatRoomResponse>(false, "show chatRoom failed", null);
        }
        return new ResponseDTO<ChatRoomResponse>(true, "show chatRoom successfully", chatRoom);

    }

    public async Task<ResponseDTO<ChatRoomResponse>> GetChatRoomByUserOneAndOne(string userOne, string userTwo)
    {
        var chatRoom = await _chatRoomRepository.GetChatRoomByUserOneAndOne(userOne, userTwo);
        if (chatRoom != null)
        {
            return new ResponseDTO<ChatRoomResponse>(true, "get chatRoom successfully", await MapToResponseDTO(chatRoom));
        }
        return new ResponseDTO<ChatRoomResponse>(false, "chatRoom not found");
    }

}
