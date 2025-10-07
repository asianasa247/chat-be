using Newtonsoft.Json;

namespace ManageEmployee.Hanet
{
    public class HanetClient
    {
        private readonly HttpClient _httpClient;

        public HanetClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<(bool isSuccess, T model)> SendRequestAsync<T>(string route, object variables, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
            }

            try
            {
                //    var query = GetQueryGraphQLByParameter<T>(route, variables);
                //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //    var queryObject = new Request
                //    {
                //        Query = query
                //    };

                //    var settings = new JsonSerializerSettings
                //    {
                //        ContractResolver = new CamelCasePropertyNamesContractResolver()
                //    };

                //    var request = new HttpRequestMessage
                //    {
                //        Method = HttpMethod.Post,
                //        Content = new StringContent(JsonConvert.SerializeObject(queryObject, settings), Encoding.UTF8, "application/json")
                //    };

                //    using var response = await _httpClient.SendAsync(request);
                //    var isSuccess = response.StatusCode == HttpStatusCode.OK;
                //    var responseString = await response.Content.ReadAsStringAsync();
                //    var result = JsonConvert.DeserializeObject<GraphQLResponse<T>>(responseString);
                //    if (!isSuccess)
                //    {
                //    }

                //    if (result.Data is null)
                //    {
                //        return (true, default);
                //    }

                return (false, default);
            }
            catch (Exception ex)
            {
                return (false, default);
            }
        }

    }
}
