using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.Entities.Enumerations.HanetEnums;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ManageEmployee.Hanet
{
    public class HanetPersonHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly AppSettingHanet _appSettingHanet;

        public HanetPersonHandler(ApplicationDbContext context, HttpClient httpClient, IOptions<AppSettingHanet> appSettingHanet)
        {
            _context = context;
            _httpClient = httpClient;
            _appSettingHanet = appSettingHanet.Value;
        }
        public async Task RemoveUser(IEnumerable<int> userIds, int placeId)
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
                if (!images.Any())
                {
                    return;
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
                        { new StringContent(user.Username),  "aliasIDs"},
                        { new StringContent(user.Username),  "placeIDs"},
                        
                    };

                    var requestUri = $"{_appSettingHanet.Endpoint}/person/removePersonByListAliasID";
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
