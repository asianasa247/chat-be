using Emgu.CV.ML;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Models.ZaloModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Org.BouncyCastle.Cms;
using PuppeteerSharp;
using System;
using System.Text;
using ZaloCSharpSDK;
using ZaloDotNetSDK;
using static System.Net.WebRequestMethods;

namespace ManageEmployee.Helpers
{
    public class ZaloModules
    {
        //secrect: 62WQ3BCr5DNokzyBPCts
        //https://oauth.zaloapp.com/v4/oa/permission?app_id=3973554442966798645&redirect_uri=
        private string BaseUrl = "https://oauth.zaloapp.com/v4/oa/";
        private string BaseUrlV3 = "https://openapi.zalo.me/v3.0/oa/";
        private string BaseUrlV2 = "https://openapi.zalo.me/v2.0/oa/";
        ZaloClient client;
        private MemoryCache memoryCache;
        private ApplicationDbContext _context;
        private MemoryCacheOptions _option;
        public ZaloModules(ApplicationDbContext _context)
        {
            _option = new MemoryCacheOptions();
            memoryCache = new MemoryCache(_option);
            this._context = _context;
        }
        public async Task<string> GetAccessToken(string appId)
        {
            var accessToken = "";
            var atObj = memoryCache.Get("zalo_auth_token");
            if (atObj == null)
            {
                var _appId = long.Parse(appId);
                var zaloConfig = _context.ZaloAppConfigs.FirstOrDefault(x => x.AppId == appId && !x.IsDelete);
                if (zaloConfig == null)
                    return null;
                if (DateTime.Now.Subtract(zaloConfig.ExpiredAt).TotalMinutes < 30)
                {
                    memoryCache.Set("zalo_auth_token", zaloConfig.AccessToken.Decrypt(), zaloConfig.ExpiredAt);
                    return zaloConfig.AccessToken.Decrypt();
                }
                var at = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(zaloConfig.AppSecret) || string.IsNullOrEmpty(zaloConfig.RefreshToken))
                {
                    if (string.IsNullOrEmpty(zaloConfig.OauthCode))
                    {

                    }
                    else
                    {
                        at = await ZaloGetAccessToken(appId, zaloConfig.AppSecret, zaloConfig.OauthCode);
                    }
                }
                else
                {
                    at = await ZaloRefreshTokenAsync(appId, zaloConfig.AppSecret, zaloConfig.RefreshToken.Decrypt());
                    
                }
                if (at == null || !at.Any())
                {
                    return null;
                }
                accessToken = at["access_token"].ToString();
                var expire = Convert.ToInt32(at["expires_in"]);
                memoryCache.Set("zalo_auth_token", accessToken, TimeSpan.FromSeconds(expire));
                zaloConfig.AccessToken = accessToken.Encrypt();
                zaloConfig.RefreshToken = at["refresh_token"].ToString().Encrypt();
                zaloConfig.ExpiredAt = DateTime.Now.AddSeconds(expire);
                _context.ZaloAppConfigs.Add(zaloConfig);
                _context.Entry(zaloConfig).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return accessToken;
            }
            accessToken = $"{atObj}";
            return accessToken.ToString();
        }
        public string GetZaloLoginUrl(string appid, string redirect_uri, string state)
        {
            return $"https://oauth.zaloapp.com/v3/auth?app_id={appid}&redirect_uri={redirect_uri}&state={state}";
        }
        public string ZaloGetAccessToken(string appid, string appsecret, string code, string redirect_uri)
        {
            return $"https://oauth.zaloapp.com/v3/access_token?app_id={appid}&app_secret={appsecret}&code={code}&redirect_uri={redirect_uri}";
        }
        public async Task<Dictionary<string, string>> ZaloRefreshTokenAsync(string appid, string appsecret, string refreshToken)
        {
            var url = "https://oauth.zaloapp.com/v4/oa/access_token";
            var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("app_id", appid),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
        });
            using (var _client = new HttpClient())
            {
                _client.DefaultRequestHeaders.Add("secret_key", appsecret);
                HttpResponseMessage response = await _client.PostAsync(url, content);
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

                    if (jsonResponse.ContainsKey("error"))
                    {
                        throw new Exception(jsonResponse["error"]);
                    }
                    return jsonResponse.ContainsKey("access_token") ? jsonResponse : null;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
        public async Task<Dictionary<string, string>> ZaloGetAccessToken(string appid, string appsecret, string codeAuth)
        {
            var url = "https://oauth.zaloapp.com/v4/oa/access_token";
            var content = new FormUrlEncodedContent(new[]
                 {
                    new KeyValuePair<string, string>("code", codeAuth),
                    new KeyValuePair<string, string>("app_id", appid),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                }); using (var _client = new HttpClient())
            {
                _client.DefaultRequestHeaders.Add("secret_key", appsecret);
                HttpResponseMessage response = await _client.PostAsync(url, content);
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

                    if (jsonResponse.ContainsKey("error"))
                    {
                        throw new Exception(jsonResponse["error_name"]);
                    }
                    return jsonResponse.ContainsKey("access_token") ? jsonResponse : null;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
        public async Task<object> GetAppInfo(string appId)
        {
            var accessToken = await GetAccessToken(appId);
            var client = new ZaloClient(accessToken).getProfileOfOfficialAccount();
            return client;
        }
        public async Task<ZaloClient> GetZaloClient(string appId)
        {
            var _appId = long.Parse(appId);

            var accessToken = await GetAccessToken(appId);
            var client = new ZaloClient(accessToken);
            return client;
        }
        public async Task<object> GetList(string appId)
        {
            var accessToken = await GetAccessToken(appId);
            var client = new ZaloClient(accessToken);
            var result = client.getListFollower(0, 100);
            return result;
        }
        public async Task<object> CreateTemplate(string appId)
        {
            var url = "https://business.openapi.zalo.me/template/create";
            var access_token = await GetAccessToken(appId);
            var content = new
            {
                name = "reminder_template",
                description = "Template nhắc nhở khách hàng về sự kiện",
                type = "notification",
                category = "reminder",
                status = "active",
                elements = new List<object>
                {
                   new  {
                content_type = "text",
                text = "📢 Xin chào {name}, \nBạn có một sự kiện: {event}vào lúc { date }tại {location}. Đừng quên tham gia nhé! 🎉"
                }, new
                {
                            content_type= "button",
                              actions= new List<object>{new  {
                                title= "Xem chi tiết",
                                  type= "oa.open.url",
                                  url= "https://yourwebsite.com"
                                }
                                }
                }
            }
            };
            var json = JsonConvert.SerializeObject(content);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (var _client = new HttpClient())
            {
                _client.DefaultRequestHeaders.Add("access_token", access_token);
                HttpResponseMessage response = await _client.PostAsync(url, data);
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<object>(responseBody);
                    return jsonResponse;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public async Task<ZaloResp<DataZaloUserResp>> GetZaloFollow(string appId, int offset, int count = 50, string from = "2025_02_15", string to = "")
        {
            var accessToken = await GetAccessToken(appId);
            if (accessToken == null)
            {
                return new ZaloResp<DataZaloUserResp>();
            }
            if (string.IsNullOrEmpty(to))
            {
                to = $"{DateTime.Now:yyyy_MM_dd}";
            }
            else
            {
                if (!DateTime.TryParse(to, out DateTime dateResult)) to = $"{DateTime.Now:yyyy_MM_dd}";
            }
            var last_interaction_period = $"{from}:{to}";
            var body = new { offset = offset, count = 50, last_interaction_period = last_interaction_period, is_follower = true };
            var url = $"{BaseUrlV3}user/getlist?data={JsonConvert.SerializeObject(body)}";
            using (var _client = new HttpClient())
            {
                _client.DefaultRequestHeaders.Add("access_token", accessToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<ZaloResp<DataZaloUserResp>>(responseBody);
                    return jsonResponse;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<ZaloResp<ZaloUserRespModel>> GetZaloDetail(string appId, string user_id)
        {
            var accessToken = await GetAccessToken(appId);
            if (accessToken == null)
            {
                return null;
            }
            var body = new { user_id = user_id };
            var url = $"{BaseUrlV3}user/detail?data={JsonConvert.SerializeObject(body)}";
            using (var _client = new HttpClient())
            {
                _client.DefaultRequestHeaders.Add("access_token", accessToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<ZaloResp<ZaloUserRespModel>>(responseBody);
                    return jsonResponse;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
        public async Task<Dictionary<string, object>> SendMessage(string appId, string user_id, string message)
        {
            var jbody = new
            {
                recipient = new
                {
                    user_id = user_id
                },
                message = new
                {
                    text = message
                }
            };
            var url = $"{BaseUrlV3}message/cs";
            using (var _client = new HttpClient())
            {
                var accessToken = await GetAccessToken(appId);
                _client.DefaultRequestHeaders.Add("access_token", accessToken);
                var data = new StringContent(JsonConvert.SerializeObject(jbody), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(url, data);
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string,object>>(responseBody);
                    return jsonResponse;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public async Task<object> GetConversion(string appId, string user_id)
        {
            var accessToken = await GetAccessToken(appId);
            var body = new { user_id = user_id, offset = 0, count = 10 };
            var url = $"{BaseUrlV2}conversation?data={JsonConvert.SerializeObject(body)}";
            using (var _client = new HttpClient())
            {
                _client.DefaultRequestHeaders.Add("access_token", accessToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                try
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<object>(responseBody);
                    return jsonResponse;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
    }
}
