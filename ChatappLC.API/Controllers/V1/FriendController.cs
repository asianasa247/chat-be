using ChatappLC.Application.Interfaces.Friend;
using ChatappLC.Infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatappLC.API.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;
        private readonly IHubContext<ChatHub> _hubContext;

        public FriendController(IFriendService friendService, IHubContext<ChatHub> hubContext)
        {
            _friendService = friendService;
            _hubContext = hubContext;
        }
        [HttpPost("request")]
        public async Task<IActionResult> SendRequest([FromQuery] string receiverId)
        {
            // ✅ Lấy userId chuẩn theo JWT Claim
            var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (senderId == null) return Unauthorized();

            var success = await _friendService.SendFriendRequestAsync(senderId, receiverId);
            if (!success) return BadRequest("Request already sent or invalid");

            // Gửi realtime tới người nhận
            await _hubContext.Clients.User(receiverId)
                .SendAsync("FriendRequestReceived", new { fromUserId = senderId });

            return Ok(true);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptRequest([FromQuery] string requestId)
        {
            // ✅ Lấy userId chuẩn theo JWT Claim
            var receiverId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (receiverId == null) return Unauthorized();

            var success = await _friendService.AcceptFriendRequestAsync(requestId, receiverId);
            if (!success) return BadRequest("Invalid request or already accepted");

            // Lấy senderId từ request
            var request = await _friendService.GetByIdAsync(requestId);
            if (request != null)
            {
                await _hubContext.Clients.User(request.SenderId)
                    .SendAsync("FriendRequestAccepted", new { fromUserId = receiverId });
            }

            return Ok(true);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();


            // 🔍 Ghi log user info
            Console.WriteLine($"[PendingList] Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"[PendingList] Claim: {claim.Type} = {claim.Value}");
            }

            if (userId == null) return Unauthorized();

            var list = await _friendService.GetPendingRequestsAsync(userId);
            return Ok(list);
        }


        [HttpGet("list")]
        public async Task<IActionResult> GetFriendList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var list = await _friendService.GetFriendListAsync(userId);
            return Ok(list); // Bây giờ list là List<UserResponse>
        }

    }
}
