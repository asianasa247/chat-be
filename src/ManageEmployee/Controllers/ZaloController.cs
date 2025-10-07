using AutoMapper;
using Emgu.CV;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Zalo;
using ManageEmployee.Helpers;
using ManageEmployee.Models.ZaloModel;
using ManageEmployee.Services.Interfaces.Zalo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloController : ControllerBase
    {
        private IZaloAppConfigService zaloAppService;
        private IZaloUserService zaloUserService;
        private ZaloModules zaloModules;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ZaloController(IZaloAppConfigService service, ApplicationDbContext _context, IZaloUserService zaloUserService, IMapper mapper)
        {
            this.zaloAppService = service;
            this.zaloModules = new ZaloModules(_context);
            this.zaloUserService = zaloUserService;
            this._context = _context;
            this._mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await zaloAppService.GetAll();
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await zaloAppService.GetById(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ZaloAppConfigModel model)
        {
            //var result = await zaloAppService.Create(model);
            if (!string.IsNullOrEmpty(model.AppSecret) && !string.IsNullOrEmpty(model.OauthCode))
            {
                var token = await zaloModules.ZaloGetAccessToken(model.AppId, model.AppSecret, model.OauthCode);
                model.AccessToken = token["access_token"].ToString().Encrypt();
                model.RefreshToken = token["refresh_token"].ToString().Encrypt();
                //model = _mapper.Map<ZaloAppConfigModel>(model);
                await zaloAppService.Create(model);
            }
            return Ok(new BaseResponseModel
            {
                Data = model
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] ZaloAppConfigModel model)
        {
            var exist = await zaloAppService.GetById(model.Id);
            if (exist == null)
            {
                return NotFound(new BaseResponseModel
                {
                    Data = "Not found"
                });
            }
            model.AccessToken = exist.AccessToken;
            model.RefreshToken = exist.RefreshToken;
            var result = await zaloAppService.Update(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await zaloAppService.Delete(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        #region Zalo Module
        [HttpGet("appInfo/{id}")]
        public async Task<IActionResult> GetAppInfoAsync(string id)
        {
            var result = await zaloModules.GetAppInfo(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("getfollower/{id}")]
        public async Task<IActionResult> GetFollowerAsync(string id, int offset = 0, int count = 50)
        {
            var result = await zaloModules.GetZaloFollow(id, offset, count);
            foreach (var item in result.Data.Users)
            {
                var zaloUser = await zaloModules.GetZaloDetail(id, item.UserId);
                if (zaloUser != null)
                {
                    var exists = await zaloUserService.GetMany(x => x.UserId == zaloUser.Data.UserId && x.UserIdByApp == zaloUser.Data.UserIdByApp);
                    var exist = exists.FirstOrDefault();
                    if (exist != null)
                    {
                        var user = _mapper.Map<ZaloUserModel>(zaloUser.Data);
                        user.Avatars = string.Join(",", zaloUser.Data.Avatars.Select(s => s.Value));
                        user.Avatar = zaloUser.Data.Avatar;
                        user.DisplayName = zaloUser.Data.DisplayName;
                        user.IsSensitive = zaloUser.Data.IsSensitive.ToString();
                        user.SharedInfo = JsonConvert.SerializeObject(zaloUser.Data.SharedInfo);
                        user.TagsAndNotesInfo = JsonConvert.SerializeObject(zaloUser.Data.TagsAndNotesInfo);
                        user.UserAlias = zaloUser.Data.UserAlias;
                        user.UserExternalId = zaloUser.Data.UserExternalId;
                        user.UserIsFollower = zaloUser.Data.UserIsFollower;
                        user.UserLastInteractionDate = zaloUser.Data.UserLastInteractionDate;
                        user.Id = exist.Id;
                        await zaloUserService.Update(user);
                    }
                    else
                    {
                        await zaloUserService.Create(new ZaloUserModel
                        {
                            Avatars = string.Join(",", zaloUser.Data.Avatars.Select(s => s.Value)),
                            Avatar = zaloUser.Data.Avatar,
                            DisplayName = zaloUser.Data.DisplayName,
                            IsSensitive = zaloUser.Data.IsSensitive.ToString(),
                            SharedInfo = JsonConvert.SerializeObject(zaloUser.Data.SharedInfo),
                            TagsAndNotesInfo = JsonConvert.SerializeObject(zaloUser.Data.TagsAndNotesInfo),
                            UserAlias = zaloUser.Data.UserAlias,
                            UserExternalId = zaloUser.Data.UserExternalId,
                            UserIsFollower = zaloUser.Data.UserIsFollower,
                            UserLastInteractionDate = zaloUser.Data.UserLastInteractionDate,
                            UserId = zaloUser.Data.UserId,
                            UserIdByApp = zaloUser.Data.UserIdByApp
                        });
                    }

                }
            }
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("getdetail/{id}")]
        public async Task<IActionResult> GetDetail(string id, string userId)
        {
            var result = await zaloModules.GetZaloDetail(id, userId);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("create_test_template/{id}")]
        public async Task<IActionResult> CreateTestTemplateAsync(string id)
        {
            var result = await zaloModules.CreateTemplate(id);

            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPost("send_message")]
        public async Task<IActionResult> SendMessageAsync([FromBody] ZaloSendMessage body)
        {
            var result = await zaloModules.SendMessage(body.AppId, body.Recipient, body.Message);
            if (result.ContainsKey("error") && result["error"].ToString()!="0")
            {
                throw new Exception(result["message"].ToString());
            }
            else
                return Ok(new BaseResponseModel
                {
                    Data = result
                });
        }
        [HttpPost("get_access_token")]
        public async Task<IActionResult> GetAccessTokenAsync([FromBody] ZaloAccessTokenOauthCodeReq model)
        {
            var result = await zaloModules.ZaloGetAccessToken(model.AppId, model.AppSecret, model.Code);

            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        #endregion
    }
}
