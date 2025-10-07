using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ManageEmployee.Emails;

public interface IEmailLogin
{
    Task GetProfileFromEmail();
}

public class EmailLogin : IEmailLogin
{
    private static async Task<UserCredential> Login(string googleClientid, string googleClientSecret, string[] scopes)
    {
        // ClientSecrets clientSecrets = new ClientSecrets()
        // {
        //     ClientId = googleClientid,
        //     ClientSecret = googleClientSecret,
        // };
        //return GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, user: "user", CancellationToken.None).Result;


        //var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //{
        //    ClientSecrets = new ClientSecrets
        //    {
        //        ClientId = googleClientid,
        //        ClientSecret = googleClientSecret
        //    },
        //    Scopes = scopes,
        //    DataStore = new FileDataStore("Store")
        //});

        //var token = new TokenResponse
        //{
        //    AccessToken = "[your_access_token_here]",
        //    RefreshToken = "[your_refresh_token_here]"
        //};

        //return new UserCredential(flow, "anhuong97@gmail.com", token);
        //string credPath = $"credentials/gmail-dotnet-credentials.json";

        //return GoogleWebAuthorizationBroker.AuthorizeAsync(
        //                      new ClientSecrets
        //                      {
        //                          ClientId = googleClientid,
        //                          ClientSecret = googleClientSecret
        //                      },
        //                      new[] { GmailService.Scope.MailGoogleCom },
        //                      "user",
        //                      CancellationToken.None,
        //                      new FileDataStore(credPath, true)).Result;


        UserCredential credential;
        using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                scopes,
                "user", CancellationToken.None, new FileDataStore("Books.ListMyLibrary"));
        }
        return credential;
        // Create the service.
        //var service = new BooksService(new BaseClientService.Initializer()
        //{
        //    HttpClientInitializer = credential,
        //    ApplicationName = "Books API Sample",
        //});

        //var bookshelves = await service.Mylibrary.Bookshelves.List().ExecuteAsync();
    }


    public static async Task GetTokenAsync(string googleClientid, string googleClientSecret, string[] scopes, string url)
    {
        HttpClient client = new();
        client.BaseAddress = new Uri("https://accounts.google.com/o/oauth2/token");
        var query = "?client_id=" + googleClientid + "&client_secret=" + googleClientSecret + "&redirect_uri=" + url + "&grant_type=authorization_code";

        var response = await client.GetAsync(query);

    }
    public async Task GetProfileFromEmail()
    {
        string googleClient = "287119297443-n0v2p6mka4dkuhm1h5r4ne94jhml5omp.apps.googleusercontent.com";
        string googleClientSecret = "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";
        string[] scopes = new[]
        {
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile",
            "https://www.googleapis.com/auth/gmail.addons.current.action.compose",
            "https://www.googleapis.com/auth/gmail.addons.current.message.action",
            "https://www.googleapis.com/auth/gmail.labels"
            //GmailService.Scope.MailGoogleCom
        };
        string url = "http://127.0.0.1:5191";

        var ii = Class1.DoIt();

        ClientSecrets clientSecrets = new ClientSecrets()
        {
            ClientId = googleClient,
            ClientSecret = googleClientSecret,
        };
        UserCredential credential = await dsAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "anhuong97@gmail.com");

        //var credential = await Login(googleClient, googleClientSecret, scopes);
        using (var gmailService = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential
        }))
        {
            var i = gmailService.Users.GetProfile("anhuong97@gmail.com").Execute();
        }
    }
}