using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.MessengerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessengerController : ControllerBase
    {
        private readonly IMessengerServices _messengerService;
        private readonly IUserService _userService;
        public MessengerController(IMessengerServices messengerService, IUserService userService)
        {
            _userService = userService;
            _messengerService = messengerService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromQuery] string userId, [FromQuery] string message)
        {
            var user = await _userService.GetByIdAsync(int.Parse(userId));

            if (user == null)
            {
                return Ok("Message sent successfully.");
            }
            var chatId = user.Note.Trim();

            if ( string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(message))
            {
                return Ok("");
            }
            try
            {
                await _messengerService.SendMessageAsync(chatId, message);
                return Ok("");
            }
            catch (Exception ex)
            {
                return Ok("");
                //return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
