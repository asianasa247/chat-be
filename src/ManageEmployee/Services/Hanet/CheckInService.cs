using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Hanets;
using Microsoft.Extensions.Options;

namespace ManageEmployee.Services.Hanet;
public class CheckInService : ICheckInService
{
    private readonly AppSettings _appSettings;

    public CheckInService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;

    }
    public async Task GetCheckinByPlaceIdInDay()
    {
        //using HttpClient client = new();
        //// set the base address of the API
        //client.BaseAddress = new Uri("https://partner.hanet.ai/");

        //// set the authorization header with your HANET API key
        //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "your_api_key_here");

        //// send a GET request to the employees API endpoint
        //var request = new
        //{
        //    token = _appSettings.AccessToken,
        //    placeID = 1,
        //    date = DateTime.Today,
        //    size = 30
        //};
        //HttpResponseMessage response = await client.PostAsJsonAsync("person/getCheckinByPlaceIdInDay", request);

        //// check if the response was successful
        //if (response.IsSuccessStatusCode)
        //{
        //    // read the response content as a string
        //    string responseContent = await response.Content.ReadAsStringAsync();

        //    // deserialize the JSON response into a list of employees
        //    List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(responseContent);

        //    // do something with the list of employees
        //    //foreach (Employee employee in employees)
        //    //{
        //    //    Console.WriteLine($"{employee.Id}: {employee.Name} ({employee.Department})");
        //    //}
        //}
        //else
        //{
        //    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        //}
    }
}
