using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ManageEmployee.Hanet
{
    public class HanetPlace : IHanetPlace
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettingHanet _appSettingHanet;

        public HanetPlace(HttpClient httpClient, IOptions<AppSettingHanet> appSettingHanet)
        {
            _httpClient = httpClient;
            _appSettingHanet = appSettingHanet.Value;
        }

        public async Task<List<SelectListModel>> GetList()
        {
            var multipartContent = new MultipartFormDataContent
            {
                { new StringContent(_appSettingHanet.AccessToken), "token" }
            };
            var requestUri = $"{_appSettingHanet.Endpoint}/place/getPlaces";
            HttpResponseMessage response = await _httpClient.PostAsync(requestUri, multipartContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseMessage = JsonConvert.DeserializeObject<HanetResponseModel<SelectListModel>>(responseContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new ErrorException(responseMessage.returnMessage);
            }
            return responseMessage.data;
        }
    }
}