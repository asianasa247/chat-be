using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace ManageEmployee.Emails;

public class dsAuthorizationBroker : GoogleWebAuthorizationBroker
{
    public static string RedirectUri = "https://accounts.google.com/o/oauth2/v2/auth?response_type=code";
    //public static string RedirectUri = "https://accounts.google.com/o/oauth2/token";

    public static async Task<UserCredential> AuthorizeAsync(
        ClientSecrets clientSecrets,
        IEnumerable<string> scopes,
        string user)
    {
        var initializer = new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
        };
        return await AuthorizeAsyncCore(initializer, scopes, user).ConfigureAwait(false);
    }

    private static async Task<UserCredential> AuthorizeAsyncCore(
        GoogleAuthorizationCodeFlow.Initializer initializer,
        IEnumerable<string> scopes,
        string user)
    {
        initializer.Scopes = scopes;
        initializer.DataStore = new FileDataStore(Folder);
        var flow = new dsAuthorizationCodeFlow(initializer);
        return await new AuthorizationCodeInstalledApp(flow,
            new LocalServerCodeReceiver())
            .AuthorizeAsync(user, CancellationToken.None).ConfigureAwait(false);
    }
}


public class dsAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
{
    public dsAuthorizationCodeFlow(Initializer initializer)
        : base(initializer) { }

    public override AuthorizationCodeRequestUrl
                   CreateAuthorizationCodeRequest(string redirectUri)
    {
        return base.CreateAuthorizationCodeRequest(dsAuthorizationBroker.RedirectUri);
    }
}
