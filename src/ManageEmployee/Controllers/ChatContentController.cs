using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.IO;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatContentController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        private readonly ApplicationDbContext _context;

        public ChatContentController(GeminiService geminiService, ApplicationDbContext context)
        {
            _geminiService = geminiService;
            _context = context;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromForm] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt không được để trống.");

            // 1. Xử lý cuộc hội thoại
            Conversation conversation;
            if (request.ConversationId == null || request.ConversationId == Guid.Empty)
            {
                conversation = new Conversation
                {
                    Title = $"Session {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }
            else
            {
                conversation = await _context.Conversations.FindAsync(request.ConversationId);
                if (conversation == null)
                    return NotFound("Không tìm thấy cuộc hội thoại.");
            }

            // 2. Lấy lịch sử hội thoại
            var previousMessages = await _context.ChatHistories
                .Where(h => h.ConversationId == conversation.Id)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();

            // 3. Chuẩn bị ảnh hoặc tài liệu
            byte[] imageBytes = null;
            string mimeType = null;
            string fileText = "";

            if (request.File != null && request.File.Length > 0)
            {
                using var ms = new MemoryStream();
                await request.File.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                if (request.File.ContentType.StartsWith("image/"))
                {
                    imageBytes = fileBytes;
                    mimeType = request.File.ContentType;
                }
                else
                {
                    fileText = Encoding.UTF8.GetString(fileBytes);
                }
            }

            // 4. Gọi Gemini API
            string responseText;
            try
            {
                if (imageBytes != null)
                {
                    // Gửi prompt + ảnh
                    var fullPrompt = request.Prompt;
                    if (!string.IsNullOrEmpty(fileText))
                        fullPrompt += $"\n\nNội dung tài liệu: {fileText}";
                    responseText = await _geminiService.GenerateContentWithImageAsync(fullPrompt, imageBytes, mimeType);
                }
                else
                {
                    // Gửi prompt + lịch sử (text-only)
                    responseText = await _geminiService.GetChatResponseWithHistoryAsync(request.Prompt, previousMessages);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

            // 5. Lưu chat history
            var history = new ChatHistory
            {
                Prompt = request.Prompt,
                Response = responseText,
                IsTrainedContent = true,
                ConversationId = conversation.Id
            };

            _context.ChatHistories.Add(history);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                conversationId = conversation.Id,
                response = history.Response,
                timestamp = history.Timestamp
            });
        }

        [HttpGet("history/{conversationId}")]
        public async Task<IActionResult> GetHistory(Guid conversationId)
        {
            var exists = await _context.Conversations.AnyAsync(c => c.Id == conversationId);
            if (!exists)
                return NotFound("Không tìm thấy cuộc hội thoại.");

            var messages = await _context.ChatHistories
                .Where(h => h.ConversationId == conversationId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var conversations = await _context.Conversations
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    FirstPrompt = _context.ChatHistories
                        .Where(h => h.ConversationId == c.Id)
                        .OrderBy(h => h.Timestamp)
                        .Select(h => h.Prompt)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(conversations);
        }
    }
}
