using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.Entities.Enumerations.HanetEnums;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ManageEmployee.Hanet
{
    public class HanetRegister : IHanetRegister
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly AppSettingHanet _appSettingHanet;

        public HanetRegister(ApplicationDbContext context, HttpClient httpClient,
            IOptions<AppSettingHanet> appSettingHanet)
        {
            _context = context;
            _httpClient = httpClient;
            _appSettingHanet = appSettingHanet.Value;
        }

        public async Task RegisterFace(IEnumerable<int> userIds, int placeId)
        {
            var users = await _context.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();
            if (!users.Any())
            {
                return;
            }

            var positions = await _context.PositionDetails.ToListAsync();
            foreach (var user in users)
            {
                var position = positions.FirstOrDefault(x => x.Id == user.PositionDetailId);
                var images = JsonConvert.DeserializeObject<List<FileDetailModel>>(user.Images.DefaultIfNullOrEmpty(""));
                if (images == null || !images.Any())
                {
                    throw new ErrorException($"User {user.FullName} chưa cung cấp ảnh");
                }
                var image = images.First();
                string path = Path.Combine(Directory.GetCurrentDirectory(), image.FileUrl);
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    var streamContent = new StreamContent(fileStream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    var multipartContent = new MultipartFormDataContent
                    {
                        { new StringContent(_appSettingHanet.AccessToken), "token" },
                        { new StringContent(user.FullName), "name" },
                        { streamContent, "file", image.FileName},
                        { new StringContent(user.Username),  "aliasID"},
                        { new StringContent(placeId.ToString()), "placeID" },
                        { new StringContent(position?.Name), "title" },
                        { new StringContent(((int)PersonTypeHanet.Staff).ToString()), "type" },
                        { new StringContent(((int)user.Gender).ToString()), "sex" },
                        { new StringContent( user.DepartmentId.ToString()),"departmentID" }
                    };

                    var requestUri = $"{_appSettingHanet.Endpoint}/person/register";
                    HttpResponseMessage response = await _httpClient.PostAsync(requestUri, multipartContent);
                    if (!response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseMessage = JsonConvert.DeserializeObject<HanetResponseModel<string>>(responseContent);
                        throw new ErrorException(responseMessage.returnMessage);
                    }
                }
            }
        }
    }
}