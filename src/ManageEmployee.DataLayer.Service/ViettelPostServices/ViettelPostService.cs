using ManageEmployee.DataTransferObject.ViettelPostModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace ManageEmployee.DataLayer.Service.ViettelPostServices;

public class ViettelPostService : IViettelPostService
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _httpClient;
    public ViettelPostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var body = new { USERNAME = username, PASSWORD = password };
        var response = await _httpClient.PostAsync($"v2/user/Login", JsonContent.Create(body));
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ViettelPostResponseApiResponse<ViettelPostLoginDataResponse>>(json, JsonOptions);
        return result.Data.Token;
    }

    public async Task<ViettelPostOrderResponse> CreateOrderAsync(ViettelPostOrderRequest request, string token)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"v2/order/createOrder")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Add("Token", token);
        var response = await _httpClient.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ViettelPostOrderResponse>(json, JsonOptions);
    }

    public async Task<ViettelPostOrderResponse> GetPriceAsync(ViettelPostOrderRequest request, string token)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"v2/order/getPrice")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Add("Token", token);
        var response = await _httpClient.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ViettelPostOrderResponse>(json, JsonOptions);
    }
}
