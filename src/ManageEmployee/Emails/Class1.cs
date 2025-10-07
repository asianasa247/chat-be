using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace ManageEmployee.Emails;

public class Class1
{
    static string clientId = "287119297443-rnd636tegjvcbcctd3a52g9me9j82o2c.apps.googleusercontent.com";
    static string clientSecret = "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";

    // The server base address
    static string baseUrl = "http://127.0.0.1:5191";
    static string accessToken = null;
    static string[] scopes = new[]
        {
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile",
            "https://www.googleapis.com/auth/gmail.addons.current.action.compose",
            "https://www.googleapis.com/auth/gmail.addons.current.message.action",
            "https://www.googleapis.com/auth/gmail.labels"
            //GmailService.Scope.MailGoogleCom
        };
    public static async Task<int> DoIt()
    {
        // Get the Access Token.
        accessToken = await GetAccessToken();
        Console.WriteLine(accessToken != null ? "Got Token" : "No Token found");

        // Get the Articles
        Console.WriteLine();
        Console.WriteLine("------ New C# Articles ------");

        dynamic response = await GetArticles(1, "c#");
        if (response.items != null)
        {
            var articles = (dynamic)response.items;
            foreach (dynamic article in articles)
                Console.WriteLine("Title: {0}", article.title);
        }

        // Get the Articles
        Console.WriteLine();
        Console.WriteLine("------ New C# Questions ------");

        response = await GetQuestions(1, "c#");
        if (response.items != null)
        {
            var questions = (dynamic)response.items;
            foreach (dynamic question in questions)
                Console.WriteLine("Title: {0}", question.title);
        }

        return 0;
    }

    /// <summary>
    /// This method uses the OAuth Client Credentials Flow to get an Access Token to provide
    /// Authorization to the APIs.
    /// </summary>
    /// <returns></returns>
    private static async Task<string> GetAccessToken()
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://accounts.google.com/o/oauth2/v2/auth");

            // We want the response to be JSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Build up the data to POST.
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            postData.Add(new KeyValuePair<string, string>("client_id", clientId));
            postData.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            postData.Add(new KeyValuePair<string, string>("redirect_uri", baseUrl));
            postData.Add(new KeyValuePair<string, string>("response_type", "code"));
            postData.Add(new KeyValuePair<string, string>("scope", "https://www.googleapis.com/auth/drive.metadata.readonly"));

            FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

            // Post to the Server and parse the response.
            HttpResponseMessage response = await client.PostAsync("Token", content);
            string jsonString = await response.Content.ReadAsStringAsync();
            object responseData = JsonConvert.DeserializeObject(jsonString);

            // return the Access Token.
            return ((dynamic)responseData).access_token;
        }
    }

    /// <summary>
    /// Gets the page of Articles.
    /// </summary>
    /// <param name="page">The page to get.</param>
    /// <param name="tags">The tags to filter the articles with.</param>
    /// <returns>The page of articles.</returns>
    private static async Task<dynamic> GetArticles(int page, string tags)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Add the Authorization header with the AccessToken.
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            // create the URL string.
            string url = string.Format("v1/Articles?page={0}&tags={1}", page, HttpUtility.UrlEncode(tags));

            // make the request
            HttpResponseMessage response = await client.GetAsync(url);

            // parse the response and return the data.
            string jsonString = await response.Content.ReadAsStringAsync();
            object responseData = JsonConvert.DeserializeObject(jsonString);
            return (dynamic)responseData;
        }
    }

    /// <summary>
    /// Gets the page of Questions.
    /// </summary>
    /// <param name="page">The page to get.</param>
    /// <param name="tags">The tags to filter the articles with.</param>
    /// <returns>The page of articles.</returns>
    private static async Task<dynamic> GetQuestions(int page, string tags)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Add the Authorization header with the AccessToken.
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            // create the URL string.
            string url = string.Format("v1/Questions/new?page={0}&include={1}", page, HttpUtility.UrlEncode(tags));

            // make the request
            HttpResponseMessage response = await client.GetAsync(url);

            // parse the response and return the data.
            string jsonString = await response.Content.ReadAsStringAsync();
            object responseData = JsonConvert.DeserializeObject(jsonString);
            return (dynamic)responseData;
        }
    }
}
