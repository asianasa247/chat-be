using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ManageEmployee.Services.Interfaces.Socials;
using Newtonsoft.Json;
using Google.Apis.Auth;
using NuGet.Protocol;
using ManageEmployee.DataTransferObject.SocialModels;

namespace ManageEmployee.Services.Socials;
public class SocialServices : ISocials
{
    private readonly HttpClient _httpClient;

    public SocialServices(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SocialVerifiedModel> GetUserInfoGoogle(string accessToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
        var userInfo = new SocialVerifiedModel
        {
            Email = payload.Email,
            EmailVerified = payload.EmailVerified,
            Name = payload.Name,
            GivenName = payload.GivenName,
            FamilyName = payload.FamilyName,
            Picture = payload.Picture,
            Locale = payload.Locale,
            Id = payload.Subject,
        };
        // Parse thông tin người dùng từ content (JSON)
        return userInfo;
    }

    public async Task<SocialVerifiedModel> GetUserInfoFacebook(string accessToken)
    {

        var response = await _httpClient.GetAsync("https://graph.facebook.com/me?fields=id,name,email,first_name,last_name,picture&access_token=" + accessToken);

        var content = await response.Content.ReadAsStringAsync();

        var json = JsonConvert.DeserializeObject<dynamic>(content);
        var userInfo = new SocialVerifiedModel
        {
            Id = json.id,
            Email = json.email,
            Name = json.name,
            GivenName = json.first_name,
            FamilyName = json.last_name,
            Picture = json.picture?.data?.url
        };
        return userInfo;
    }
}

