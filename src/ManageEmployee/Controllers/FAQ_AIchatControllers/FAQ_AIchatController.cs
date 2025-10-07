using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.FAQ_AIchat;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.FAQ_AI_Chatbot;
using ManageEmployee.Services.CompanyServices;
using ManageEmployee.Services.FAQ_AIchatServices;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.FAQ_AIchat;
using ManageEmployee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace ManageEmployee.Controllers.FAQ_AIchatControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FAQ_AIchatController : ControllerBase
    {
        private readonly ILogger<FAQ_AIchatController> _logger;
        private readonly IFAQ_AIchatService _faqAIchatService;
        private readonly ICompanyService _companyService;
        private readonly IFAQ_AIchatDetailService _faqAIchatDetailService;

        public FAQ_AIchatController(ILogger<FAQ_AIchatController> logger,
            IFAQ_AIchatService faqAIchatService,
            ICompanyService companyService,
            IFAQ_AIchatDetailService fAQ_AIchatDetailService)
        {
            _logger = logger;
            _faqAIchatService = faqAIchatService;
            _companyService = companyService;
            _faqAIchatDetailService = fAQ_AIchatDetailService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel model)
        {
            var resultLogs = await _faqAIchatService.GetAll(model.Page, model.PageSize);
            var totalItems = await _faqAIchatService.TotalChat();
            var responeData = new List<FAQChatViewModel>();

            foreach (var chat in resultLogs)
            {
                var chatDetails = await _faqAIchatDetailService.GetAllbyChatId(chat.Id);

                responeData.Add(new FAQChatViewModel
                {
                    Id = chat.Id,
                    UserId = chat.UserId,
                    CreateAt = chat.CreateAt,
                    UpdateAt = chat.UpdateAt,
                    Department = chat.Department,
                    FirstTopic = chat.FirstTopic,
                    ChatDetails = chatDetails
                });
            }

            return Ok(new BaseResponseModel
            {
                Data = responeData,
                CurrentPage = model.Page,
                PageSize = model.PageSize,
                TotalItems = totalItems,
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FAQ_AIchatModel model)
        {
            var result = await _faqAIchatService.Create(model);
            return Ok(new { id = result.Id });
        }

        [HttpPut("edit")]
        public async Task<IActionResult> Edit([FromBody] FAQ_AIchatModel model)
        {
            try
            {
                var result = await _faqAIchatService.Update(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _faqAIchatService.Delete(id);
            if (result)
                return Ok(new { msg = "Delete success" });
            return NotFound(new { msg = "FAQ_AIchat not found" });
        }

        [HttpDelete("deleteDetail/{id}")]
        public async Task<IActionResult> DeleteDetail(int id)
        {
            var result = await _faqAIchatDetailService.Delete(id);
            if (result)
                return Ok(new { msg = "Delete success" });
            return NotFound(new { msg = "FAQ_AIchatDetail not found" });
        }

        public class QuestionRequest
        {
            public int FaqAIchatId { get; set; }
            public string Question { get; set; }
        }
        private async Task<string> AnswerQuestionWithGeminiAsync(string question)
        {
            Company company = await _companyService.GetCompany();
            if (company == null)
                throw new Exception("Company not found.");

            //string geminiApiKey = company.GoogleAppId;
            string geminiApiKey = "AIzaSyAfU_HexIDiF2j - W6SJoMpf4Gygo1claTE";

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
            httpClient.DefaultRequestHeaders.Add("x-goog-api-key", geminiApiKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = question }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await httpClient.PostAsync("v1beta/models/gemini-2.0-flash:generateContent", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error ({response.StatusCode}): {errorContent}");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            // ✅ Parse JSON để lấy text trả về
            using var doc = JsonDocument.Parse(responseString);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text?.Trim() ?? string.Empty;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(new { msg = "Question cannot be empty." });
            try
            {
                var answer = await AnswerQuestionWithGeminiAsync(request.Question);
                await _faqAIchatDetailService.Create(new FAQ_AIchatDetailModel
                {
                    FAQ_AIchatId = request.FaqAIchatId,
                    Question = request.Question,
                    Answer = answer,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                });
                return Ok(new { answer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling Gemini API.");
                return StatusCode(500, new { msg = "Error while processing the request." });
            }
        }

        #region Commented Code for Future Reference
        //public class QuestionRequest
        //{
        //    public int FaqAIchatId { get; set; }
        //    public string Question { get; set; }
        //}

        //[HttpPost("ask")]
        //public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.Question))
        //        return BadRequest(new { msg = "Question cannot be empty." });

        //    try
        //    {
        //        // Truy vấn history từ database
        //        var history = await _faqAIchatDetailService.GetAllbyChatId(request.FaqAIchatId);

        //        // Xây contents
        //        var contents = history.SelectMany(h => new[]
        //        {
        //            new {
        //                role = "user",
        //                parts = new[] { new { text = h.Question } }
        //            },
        //            new {
        //                role = "model",
        //                parts = new[] { new { text = h.Answer } }
        //            }
        //        }).ToList();

        //        // Thêm câu hỏi mới
        //        contents.Add(new
        //        {
        //            role = "user",
        //            parts = new[] { new { text = request.Question } }
        //        });

        //        // Gửi Gemini
        //        var answer = await CallGeminiWithContents(contents);

        //        // Lưu lại lịch sử
        //        var newDetail = new FAQ_AIchatDetail
        //        {
        //            FAQ_AIchatId = request.FaqAIchatId,
        //            Question = request.Question,
        //            Answer = answer,
        //            CreateAt = DateTime.UtcNow,
        //            UpdateAt = DateTime.UtcNow
        //        };
        //        // Convert FAQ_AIchatDetail entity to FAQ_AIchatDetailModel before calling Create
        //        var newDetailModel = new FAQ_AIchatDetailModel
        //        {
        //            FAQ_AIchatId = newDetail.FAQ_AIchatId,
        //            CreateAt = newDetail.CreateAt
        //            // Add other properties as needed if FAQ_AIchatDetailModel has more fields
        //        };
        //        await _faqAIchatDetailService.Create(newDetailModel);

        //        return Ok(new { answer });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while calling Gemini API.");
        //        return StatusCode(500, new { msg = "Error while processing the request." });
        //    }
        //}
        //private async Task<string> CallGeminiWithContents(object contents)
        //{
        //    Company company = await _companyService.GetCompany();
        //    if (company == null)
        //        throw new Exception("Company not found.");

        //    string geminiApiKey = company.GoogleAppId;

        //    using var httpClient = new HttpClient();
        //    httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
        //    httpClient.DefaultRequestHeaders.Add("x-goog-api-key", geminiApiKey);
        //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

        //    var requestBody = new { contents };

        //    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, MediaTypeNames.Application.Json);

        //    var response = await httpClient.PostAsync("v1beta/models/gemini-2.0-flash:generateContent", content);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var errorContent = await response.Content.ReadAsStringAsync();
        //        throw new Exception($"Gemini API Error ({response.StatusCode}): {errorContent}");
        //    }

        //    var responseString = await response.Content.ReadAsStringAsync();

        //    using var doc = JsonDocument.Parse(responseString);
        //    var text = doc.RootElement
        //        .GetProperty("candidates")[0]
        //        .GetProperty("content")
        //        .GetProperty("parts")[0]
        //        .GetProperty("text")
        //        .GetString();

        //    return text?.Trim() ?? string.Empty;
        //}
        #endregion
    }
}
