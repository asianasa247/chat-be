using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Entities; // <-- THÊM DÒNG NÀY ĐỂ TRUY CẬP ĐẾN ENTITY ScheduledPost
using System; // Đảm bảo có
using System.IO; // Đảm bảo có
using System.Threading.Tasks; // Đảm bảo có
using System.Linq; // Đảm bảo có cho .Select()

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookScheduleController : ControllerBase
    {
        private readonly IScheduledPostService _scheduledPostService;
        private readonly IFaceAccessService _faceAccessService;
        private readonly ApplicationDbContext _dbContext;

        public FacebookScheduleController(
            IScheduledPostService scheduledPostService,
            IFaceAccessService faceAccessService,
            ApplicationDbContext dbContext)
        {
            _scheduledPostService = scheduledPostService;
            _faceAccessService = faceAccessService;
            _dbContext = dbContext;
        }

        [HttpPost("schedule-post")]
        public async Task<IActionResult> SchedulePost([FromForm] SchedulePostRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest("Message không được để trống.");

            if ((request.Photo == null || request.Photo.Length == 0) && string.IsNullOrEmpty(request.ImageUrl))
                return BadRequest("Phải cung cấp ảnh upload hoặc đường dẫn URL ảnh.");

            if (request.ScheduledTime <= DateTime.Now)
                return BadRequest("Thời gian đặt lịch phải lớn hơn hiện tại.");

            var access = await _dbContext.FaceAccesses.FirstOrDefaultAsync(f => f.Id == request.AccessId && f.IsActive);
            if (access == null)
                return BadRequest("Không tìm thấy Page Access Token đang hoạt động tương ứng với accessId.");

            byte[]? photoBytes = null;
            string? fileName = null;
            string? contentType = null;

            if (request.Photo != null && request.Photo.Length > 0)
            {
                using var ms = new MemoryStream();
                await request.Photo.CopyToAsync(ms);
                photoBytes = ms.ToArray();
                fileName = request.Photo.FileName;
                contentType = request.Photo.ContentType;
            }

            var scheduledPostEntity = new ManageEmployee.Entities.ScheduledPost
            {
                Id = Guid.NewGuid(),
                Message = request.Message,
                PhotoData = photoBytes,
                PhotoFileName = fileName,
                PhotoContentType = contentType,
                ImageUrl = request.ImageUrl,
                ScheduledTime = request.ScheduledTime,
                RepeatType = request.RepeatType?.ToLower() ?? "none",
                IsActive = true,
                PageId = access.PageId,
                PageAccessToken = access.PageAccessToken
            };

            await _scheduledPostService.AddScheduledPostAsync(scheduledPostEntity);

            return Ok(new
            {
                message = "Đã đặt lịch đăng bài!",
                scheduledPostId = scheduledPostEntity.Id,
                pageAccessToken = scheduledPostEntity.PageAccessToken
            });
        }


        [HttpGet("scheduled-posts")]
        public async Task<IActionResult> GetScheduledPosts()
        {
            var posts = await _scheduledPostService.GetAllScheduledPostsAsync();

            var result = posts.Select(p => new
            {
                p.Id,
                p.Message,
                p.PhotoFileName,
                p.ScheduledTime,
                p.RepeatType,
                p.IsActive,
                PostedTime = p.PostedTime?.ToLocalTime()
            });

            return Ok(result);
        }

        [HttpDelete("scheduled-post/{id}")]
        public async Task<IActionResult> CancelScheduledPost(Guid id)
        {
            var post = await _scheduledPostService.GetScheduledPostByIdAsync(id);

            if (post == null)
                return NotFound("Không tìm thấy lịch đăng bài này.");

            if (!post.IsActive)
                return BadRequest("Lịch đăng bài đã bị hủy trước đó.");

            post.IsActive = false;
            await _scheduledPostService.UpdateScheduledPostAsync(post);

            return Ok(new { message = "Đã hủy lịch đăng bài." });
        }
    }
}