using ManageEmployee.DataTransferObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageGenerationController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiKey = "jzzMZAropg7w0jEg9ePDMuaSOouBALgq";
        private const string Endpoint = "https://api.runware.ai/v1";

        public ImageGenerationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateImage([FromBody] PromptRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            var taskUUID = Guid.NewGuid().ToString();

            var payload = new RunwareRequestItem[]
            {
                new RunwareRequestItem
                {
                    TaskType = "authentication",
                    ApiKey = ApiKey
                },
                new RunwareRequestItem
                {
                    TaskType = "imageInference",
                    TaskUUID = taskUUID,
                    PositivePrompt = request.Prompt,
                    Width = 512,
                    Height = 512,
                    Model = "civitai:102438@133677",
                    NumberResults = 1
                }
            };

            var jsonBody = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(Endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, error);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);

            if (!doc.RootElement.TryGetProperty("data", out var dataArray))
            {
                return BadRequest(new { error = "No data in response", raw = jsonResponse });
            }

            var imageResult = dataArray.EnumerateArray()
                .FirstOrDefault(x => x.GetProperty("taskType").GetString() == "imageInference" &&
                                     x.GetProperty("taskUUID").GetString() == taskUUID);

            if (imageResult.ValueKind == JsonValueKind.Undefined)
            {
                return BadRequest(new { error = "Image generation result not found", raw = jsonResponse });
            }

            var imageUrl = imageResult.GetProperty("imageURL").GetString();
            return Ok(new { url = imageUrl });
        }
    }
}
