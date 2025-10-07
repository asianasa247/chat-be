using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookApiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IFaceAccessService _faceAccessService;

        public FacebookApiController(HttpClient httpClient, IFaceAccessService faceAccessService)
        {
            _httpClient = httpClient;
            _faceAccessService = faceAccessService;
        }

        [HttpPost("post-with-upload")]
        public async Task<IActionResult> PostWithUpload([FromForm] PostWithUploadRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest("Message không được để trống.");

            if (request.AccessId == Guid.Empty)
                return BadRequest("AccessId không hợp lệ.");

            var access = await _faceAccessService.GetAccessByIdAsync(request.AccessId);
            if (access == null)
                return NotFound("Không tìm thấy thông tin access với AccessId đã chọn.");

            var postUrl = $"https://graph.facebook.com/{access.PageId}/photos?access_token={access.PageAccessToken}";

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.Message), "caption");

            if (request.Photo != null && request.Photo.Length > 0)
            {
                // Upload từ file
                using var stream = request.Photo.OpenReadStream();
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.Photo.ContentType);
                content.Add(streamContent, "source", request.Photo.FileName);
            }
            else if (!string.IsNullOrEmpty(request.ImageUrl))
            {
                // Upload từ URL
                content.Add(new StringContent(request.ImageUrl), "url");
            }
            else
            {
                return BadRequest("Cần chọn 1 trong 2: file ảnh hoặc URL ảnh.");
            }

            var postResponse = await _httpClient.PostAsync(postUrl, content);
            var postContent = await postResponse.Content.ReadAsStringAsync();

            if (!postResponse.IsSuccessStatusCode)
                return StatusCode((int)postResponse.StatusCode, new { error = postContent });

            return Ok(new { message = "Đã đăng bài có ảnh lên page!", facebookResponse = JObject.Parse(postContent) });
        }

        [HttpPost("add-access")]
        public async Task<IActionResult> AddAccess([FromBody] FaceAccess request)
        {
            if (string.IsNullOrWhiteSpace(request.PageId) || string.IsNullOrWhiteSpace(request.PageAccessToken))
                return BadRequest("PageId và PageAccessToken không được để trống.");

            var newAccess = new FaceAccess
            {
                PageId = request.PageId,
                PageAccessToken = request.PageAccessToken,
                IsActive = request.IsActive
            };

            var result = await _faceAccessService.AddAccessAsync(newAccess);

            if (!result)
                return StatusCode(500, "Lỗi khi lưu thông tin vào cơ sở dữ liệu.");

            return Ok(new { message = "Đã thêm thông tin Page Access thành công!", data = newAccess });
        }
        [HttpGet("access-list")]
        public async Task<IActionResult> GetAccessList()
        {
            var list = await _faceAccessService.GetAllAsync();

            var result = list.Select(f => new
            {
                f.Id,
                f.PageId,
                f.PageAccessToken,
                f.IsActive
            });

            return Ok(result);
        }

        [HttpPut("update-access/{id}")]
        public async Task<IActionResult> UpdateAccess(Guid id, [FromBody] FaceAccess updated)
        {
            var existing = await _faceAccessService.GetAccessByIdAsync(id);
            if (existing == null)
                return NotFound("Không tìm thấy Access cần cập nhật.");

            existing.PageId = updated.PageId ?? existing.PageId;
            existing.PageAccessToken = updated.PageAccessToken ?? existing.PageAccessToken;
            existing.IsActive = updated.IsActive;

            var result = await _faceAccessService.UpdateAccessAsync(existing);

            if (!result)
                return StatusCode(500, "Lỗi khi cập nhật thông tin.");

            return Ok(new { message = "Đã cập nhật thành công!", data = existing });
        }

        [HttpDelete("delete-access/{id}")]
        public async Task<IActionResult> DeleteAccess(Guid id)
        {
            var access = await _faceAccessService.GetAccessByIdAsync(id);
            if (access == null)
                return NotFound("Không tìm thấy Access để xóa.");

            access.IsActive = false;

            var result = await _faceAccessService.UpdateAccessAsync(access);
            if (!result)
                return StatusCode(500, "Lỗi khi xóa (ẩn) Access.");

            return Ok(new { message = "Đã ẩn access thành công." });
        }


        public class PostWithUploadRequest
        {
            public string Message { get; set; }
            public IFormFile? Photo { get; set; }
            public string? ImageUrl { get; set; }
            public Guid AccessId { get; set; }
        }
    }
}
