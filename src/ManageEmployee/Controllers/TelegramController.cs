using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly IUserService _userService;
        public TelegramController(ITelegramBotService telegramBotService, IUserService userService)
        {
            _userService = userService;
            _telegramBotService = telegramBotService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromQuery] string userId, [FromQuery] string message)
        {
            var user = await _userService.GetByIdAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var chatId = user.Note.Trim();
            if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(message))
            {
                return BadRequest("Chat ID and message cannot be empty.");
            }
            try
            {
                await _telegramBotService.SendMessageAsync(chatId, message);
                return Ok("Message sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
