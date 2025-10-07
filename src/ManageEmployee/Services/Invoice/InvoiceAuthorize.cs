using ManageEmployee.DataTransferObject;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Invoices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ManageEmployee.Services.Invoice;
public class InvoiceAuthorize: IInvoiceAuthorize
{
    private readonly AppSettingInvoice _appSettingInvoice;

    public InvoiceAuthorize(IOptions<AppSettingInvoice> appSettingInvoice)
    {
        _appSettingInvoice = appSettingInvoice.Value;
    }
    public async Task<string> PerformAsync()
    {
        using HttpClient client = new();
        // set the base address of the API
        client.BaseAddress = new Uri(_appSettingInvoice.Endpoint);
        AuthenticateModel request = new()
        {
            Username = "0100109106-718",
            Password = "123456aA@"
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/auth/login", request);

        // check if the response was successful
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            LoginInvoiceResponse res = JsonConvert.DeserializeObject<LoginInvoiceResponse>(responseContent);
            return res.AccessToken;
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }
        return "";
    }
}
public class LoginInvoiceResponse
{
[JsonProperty("access_token")]
public string? AccessToken { get; set; }
}
