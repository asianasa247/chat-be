namespace ChatappLC.Infrastructure.Services.Chat;

internal class ChatService : BaseService, IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IChatRoomService _chatRoomService;

    public ChatService(IChatRepository chatRepository,
                       IChatRoomService chatRoomService, IHttpContextAccessor httpContextAccessor)
        : base(httpContextAccessor)
    {
        _chatRepository = chatRepository;
        _chatRoomService = chatRoomService;
    }

    public async Task<ResponseDTO<ChatMessageResponse>> CreateChatMessageAsync(ChatMessageRequest createDTO)
    {
        var room = await _chatRoomService.GetChatRoomByIdAsync(createDTO.ChatRoomId);
        if (room.Data == null)
        {
            return new ResponseDTO<ChatMessageResponse>(false, "Chat room not found");
        }

        var sender = await _chatRoomService.GetUserInRoomAsync(createDTO.ChatRoomId, createDTO.SenderId);
        if (!sender)
        {
            return new ResponseDTO<ChatMessageResponse>(false, "Sender not found in chat room");
        }

        var chatMessage = ChatMessage.Create(createDTO.SenderId, createDTO.ChatRoomId, createDTO.Content, createDTO.MessageType, createDTO.AttachmentUrl, createDTO.FileSize);

        var createdMessage = await _chatRepository.CreateChatMessageAsync(chatMessage);
        var result = ChatMessageResponse.MapToResponseDTO(createdMessage);
        // Cập nhật thông tin tin nhắn cuối cùng trong ChatRoom
        var lastMessages = createDTO.MessageType == "text" ? createDTO.Content : createDTO.MessageType;
        var chatRoomRequest = RoomRequest(createDTO.ChatRoomId, lastMessages);
        await _chatRoomService.UpdateChatRoomAsync(chatRoomRequest);
        return new ResponseDTO<ChatMessageResponse>(true, "gửi tin nhắn thành công", result);
    }

    public async Task<ResponseDTO<PagedResult<ChatMessageResponse>>> GetMessagesByRoomIdAsync(string chatRoomId, int currentPage, int pageSize)
    {
        var entities = await _chatRepository.GetChatMessagesByRoomIdAsync(chatRoomId);
        var allMessages = entities.Select(ChatMessageResponse.MapToResponseDTO).ToList();

        int totalCount = allMessages.Count;
        int skip = (currentPage - 1) * pageSize;
        var items = allMessages
            .OrderByDescending(m => m.Timestamp)
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<ChatMessageResponse>
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };

        return new ResponseDTO<PagedResult<ChatMessageResponse>>(true, "Lấy message thành công", result);
    }

    public async Task<ResponseDTO<ChatMessageResponse>> RecallMessageAsync(string messageId)
    {
        var message = await _chatRepository.GetChatMessageByIdAsync(messageId);
        if (message == null)
        {
            return new ResponseDTO<ChatMessageResponse>(false, "Message not found");
        }

        message.UpdateStatus("Recalled");

        await _chatRepository.UpdateMessageAsync(message);
        // Update the last message in the chat room
        var chatRoomRequest = RoomRequest(message.ChatRoomId, "recalled");
        await _chatRoomService.UpdateChatRoomAsync(chatRoomRequest);

        return new ResponseDTO<ChatMessageResponse>(true, "Message recalled successfully", ChatMessageResponse.MapToResponseDTO(message));
    }

    public async Task<ResponseDTO<ChatMessageResponse>> DeleteMessageAsync(string messageId)
    {
        var message = await _chatRepository.GetChatMessageByIdAsync(messageId);
        if (message == null)
        {
            return new ResponseDTO<ChatMessageResponse>(false, "Message not found");
        }
        await _chatRepository.DeleteMessageAsync(messageId);

        var lastMessageUpdate = string.Empty;
        var lastMessages = await _chatRepository.GetChatMessagesByRoomIdAsync(message.ChatRoomId);
        var lastMessage = lastMessages.OrderByDescending(m => m.Timestamp).FirstOrDefault();
        lastMessageUpdate = lastMessage?.Status == "Recall" ? "recalled."
            : lastMessage?.MessageType == "text" ? lastMessage?.Content : lastMessage?.MessageType;
        await _chatRoomService.UpdateChatRoomAsync(RoomRequest(message.ChatRoomId, lastMessageUpdate ?? ""));
        return new ResponseDTO<ChatMessageResponse>(true, "Message deleted successfully", ChatMessageResponse.MapToResponseDTO(message));
    }

    private ChatRoomUpdate RoomRequest(string roomId, string lastMessage)
    {
        return new ChatRoomUpdate
        {
            Id = roomId,
            RoomName = string.Empty,
            LastMessage = lastMessage
        };
    }

}
