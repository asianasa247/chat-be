using ManageEmployee.DataTransferObject.CompanyModels;
using ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels;
using ManageEmployee.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ManageEmployee.HttpClients
{
    public class VitaxOneClient : IVitaxOneClient
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettingVintaxInvoice _appSettingVintaxInvoice;

        public VitaxOneClient(HttpClient httpClient, IOptions<AppSettingVintaxInvoice> appSettingVintaxInvoice)
        {
            _httpClient = httpClient;
            _appSettingVintaxInvoice = appSettingVintaxInvoice.Value;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string uri)
        {
            var request = new HttpRequestMessage(method, new Uri(new Uri(_appSettingVintaxInvoice.Endpoint), uri));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _appSettingVintaxInvoice.AccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return request;
        }

        public async Task<VintaxInvoiceInResponse<T>> GetAsync<T>(string uri, string taxCode, string passwordAccountTax)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(180)); // Timeout mềm 3 phút

            var request = CreateRequest(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(request,cts.Token);

            var json = await response.Content.ReadAsStringAsync();

            var responseModel = JsonConvert.DeserializeObject<VintaxInvoiceInResponse<T>>(json);
            if (responseModel.status.Contains("Fail"))
            {
                throw new ErrorException("Lỗi không lấy được dữ liệu");
            }

            return responseModel;
        }

        public async Task<OtherCompanyInfomationModel> GetCompanyInfoAsync(string uri, string taxCode)
        {
            var request = CreateRequest(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<OtherCompanyInfomationModel>(json);

            if (responseModel is null)
            {
                await LoginTCT(taxCode);

                request = CreateRequest(HttpMethod.Get, uri);
                response = await _httpClient.SendAsync(request);
                json = await response.Content.ReadAsStringAsync();
                responseModel = JsonConvert.DeserializeObject<OtherCompanyInfomationModel>(json);
            }

            return responseModel;
        }

        private async Task LoginTCT(string taxCode)
        {
            var uri = $"Invoices/LoginTCT?mst={taxCode}";
            var request = CreateRequest(HttpMethod.Get, uri);
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            var responseModel = JsonConvert.DeserializeObject<VintaxInvoiceInCommonResponse>(json);
            if (responseModel.status == "255")
            {
                throw new Exception(ErrorMessages.LOGIN_AGAIN);
            }
        }

        public async Task Login(string newPassword, string taxCode)
        {
            var form = new VitaxOneLoginModel
            {
                username = taxCode,
                password = newPassword
            };

            var requestUri = new Uri(new Uri(_appSettingVintaxInvoice.Endpoint), "Invoices/login_tct_client");

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(form)
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _appSettingVintaxInvoice.AccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<VintaxInvoiceInCommonResponse>(json);

            if (responseModel.status.Contains("Fail"))
            {
                throw new Exception(responseModel.status);
            }
        }
    }
}