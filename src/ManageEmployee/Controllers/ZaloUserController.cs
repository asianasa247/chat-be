using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Zalo;
using ManageEmployee.Entities.ZaloEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Models.ZaloModel;
using ManageEmployee.Services.Interfaces.Zalo;
using ManageEmployee.Services.ZaloServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloUserController : ControllerBase
    {
        private readonly IZaloUserService _zaloUserService;
        private readonly IZaloAppConfigService _zaloAppConfigService;
        private ZaloModules zaloModules;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ZaloUserController(IZaloUserService zaloUserService, IZaloAppConfigService zaloAppConfigService, ApplicationDbContext context, IMapper mapper)
        {
            _zaloUserService = zaloUserService;
            _zaloAppConfigService = zaloAppConfigService;
            zaloModules = new ZaloModules(context);
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFollows(string appId, int offset, int count = 50)
        {
            var appLong = long.Parse(appId);
            var dataFollow = await zaloModules.GetZaloFollow(appId, offset, count);

            // Kiểm tra lỗi từ response của Zalo
            if (dataFollow == null || dataFollow.Data == null || dataFollow.Error != 0)
            {
                return BadRequest(new BaseResponseModel
                {
                    DataTotal = dataFollow?.Message ?? "Không thể kết nối đến Zalo API.",
                });
            }

            var user_follow_ids = dataFollow.Data.Users.Select(s => { long.TryParse(s.UserId, out var userId); return userId; }).ToList();
            var users = (await _zaloUserService.GetMany(z =>z.AppId == appLong && user_follow_ids.Contains(z.UserId))).ToList();
            var userNotInDb = dataFollow.Data.Users.Where(s => !users.Any(a => a.UserId.ToString() == s.UserId)).ToList();
            foreach (var item in userNotInDb)
            {
                var zaloUser = await zaloModules.GetZaloDetail(appId, item.UserId);
                if (zaloUser != null)
                {
                    var user = new ZaloUserModel
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
                        UserIdByApp = zaloUser.Data.UserIdByApp, AppId = appLong
                    };
                    await _zaloUserService.Create(user);
                    var userMap = _mapper.Map<ZaloUser>(user);
                    users.Add(userMap);
                }
            }
            var resultData = users.Select(s => new ZaloUserModelV2
            {
                UserId = s.UserId,
                UserIdByApp = s.UserIdByApp,
                UserExternalId = s.UserExternalId,
                DisplayName = s.DisplayName,
                UserAlias = s.UserAlias,
                IsSensitive = s.IsSensitive,
                UserLastInteractionDate = s.UserLastInteractionDate,
                UserIsFollower = s.UserIsFollower,
                Avatar = s.Avatar,
                Avatars = s.Avatars,
                TagsAndNotesInfo = s.TagsAndNotesInfo,
                SharedInfo = s.SharedInfo,
                AppId = s.AppId
            });
            return Ok(new BaseResponseModel
            {
                Data = resultData
            });
        }
        [HttpGet("get_conversion/{id}")]
        public async Task<IActionResult> GetConversion(string id, string userId)
        {
            var result = await zaloModules.GetConversion(id, userId);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
    }
}
