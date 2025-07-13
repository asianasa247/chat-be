
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatappLC.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class ChatRoomController : BaseController
{
    private readonly IChatService _chatService;
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomController(IChatService chatService, IChatRoomService chatRoomService)
    {
        _chatService = chatService;
        _chatRoomService = chatRoomService;
    }

    // Endpoint lấy danh sách tin nhắn theo phòng chat
    [HttpGet("messages")]
    [Authorize]
    public async Task<IActionResult> GetMessagesByRoomId([FromQuery] string roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = await _chatService.GetMessagesByRoomIdAsync(roomId, page, pageSize);
        return HandleResponse(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateChatRoom([FromBody] ChatRoomRequest createDTO)
    {
        var room = await _chatRoomService.CreateChatRoomAsync(createDTO);
        return HandleResponse(room);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetChatRoomsByUserId()
    {
        var rooms = await _chatRoomService.GetChatRoomsByUserIdAsync();
        return HandleResponse(rooms);
    }

    [HttpPost("one-one")]
    public async Task<IActionResult> showRoom([FromBody] ChatRoomRequest createDTO)
    {
        var room = await _chatRoomService.ShowRoomOrCreateRoomForOneAndOneAsync(createDTO);
        return HandleResponse(room);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateChatRoom([FromBody] ChatRoomUpdate chatRoomDTO)
    {
        await _chatRoomService.UpdateChatRoomAsync(chatRoomDTO);
        return NoContent();
    }

}
