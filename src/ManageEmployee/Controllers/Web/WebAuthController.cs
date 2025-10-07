using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Services.Interfaces.Webs;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Socials;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces.Customers;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;

namespace ManageEmployee.Controllers.Web;

[ApiController]
[Route("api/[controller]")]
public class WebAuthController : ControllerBase
{
    private readonly IWebAuthService _webAuthService;
    private readonly ISocials _socialServices;
    private readonly ICustomerService _customerService;

    public WebAuthController(
         IWebAuthService webAuthService,
         ISocials socialServices,
         ICustomerService customerService
         )
    {
        _webAuthService = webAuthService;
        _socialServices = socialServices;
        _customerService = customerService;
    }

    /// <summary>
    /// Đăng nhập web
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthenticateModel model)
    {
        var user = await _webAuthService.Authenticate(model.Username, model.Password);

        var authClaims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName", user.Phone),
                    new Claim("Name",  !String.IsNullOrEmpty(user.Name)? user.Name : ""),
                };

        var tokenString = _webAuthService.GenerateToken(authClaims);

        return Ok(new
        {
            Id = user.Id,
            Username = user.Code,
            Fullname = user.Name,
            Avatar = user.Avatar,
            Token = tokenString,
            Email = user.Email,
            Phone = user.Phone,
        });
    }

    /// <summary>
    /// Đăng nhập web
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("loginsocial")]
    public async Task<IActionResult> LoginSocial(AuthenticateSocialModel model)
    {
        var user = await _webAuthService.RegisterAccountSocial(model);
        var authClaims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName", user.Phone),
                    new Claim("Name",  !String.IsNullOrEmpty(user.Name)? user.Name : ""),
                };

        var tokenString = _webAuthService.GenerateToken(authClaims);

        return Ok(new
        {
            Id = user.Id,
            Username = user.Code,
            Fullname = user.Name,
            Avatar = user.Avatar,
            Token = tokenString,
            Email = user.Email,
            Phone = user.Phone,
            IsLoginSocial = true
        });
    }

    /// <summary>
    /// Đăng ký tài khoản
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(WebCustomerV2Model model)
    {
        var user = await _webAuthService.Register(model);
        var authClaims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserName", user.Phone),
                    new Claim("Name",  !String.IsNullOrEmpty(user.Name)? user.Name : ""),
                };

        var tokenString = _webAuthService.GenerateToken(authClaims);

        return Ok(new ObjectReturn
        {
            status = 200,
            data = new
            {
                Id = user.Id,
                Code = user.Code,
                Name = user.Name,
                Avatar = user.Avatar,
                Phone = user.Phone,
                Token = tokenString,
            }
        });
    }

    /// <summary>
    /// Update email
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("update-email")]
    public async Task<IActionResult> UpdateEmail(WebCustomerV2Model model)
    {
        await _webAuthService.UpdateMail(model);
        return Ok();
    }


    /// <summary>
    /// login google,fb
    /// </summary>
    [HttpPost("login-social")]
    public async Task<IActionResult> LoginSocial(SocialModel model)
    {
        var userInfo = model.Provider switch
        {
            "FACEBOOK" => await _socialServices.GetUserInfoFacebook(model.Token),
            "GOOGLE" => await _socialServices.GetUserInfoGoogle(model.Token),
            _ => throw new ArgumentException("Invalid provider", nameof(model.Provider))
        };
        Console.Write(userInfo.ToJson() + "<<<<<<<<<<<<< userInfo");

        var userQuery = await this._customerService.GetBySocialProviderId(model.Provider, userInfo.Id);

        var authSocialModel = new AuthenticateSocialModel
        {
            Email = userInfo.Email,
            Name = userInfo.Name,
            Avarta = userInfo.Picture,
            Provider = model.Provider,
            PhotoUrl = userInfo.Picture,
            Gender = GenderEnum.All,
            ProviderId = userInfo.Id
        };

        if (userQuery == null)
        {
            userQuery = await _webAuthService.RegisterAccountSocial(authSocialModel);
        }

        var authClaims = new List<Claim>
                {
                    new Claim("UserId", userQuery.Id.ToString()),
                    new Claim("ProviderId", userQuery.ProviderId),
                    new Claim("Name",  !String.IsNullOrEmpty(userQuery.Name)? userQuery.Name : ""),
                };

        var tokenString = _webAuthService.GenerateToken(authClaims);

        return Ok(new
        {
            Username = userQuery.Code,
            Fullname = userQuery.Name,
            Avatar = userQuery.Avatar,
            Token = tokenString,
            Email = userQuery.Email,
            Phone = userQuery.Phone,
            IsLoginSocial = true,
            Id = userQuery.Id
        });
    }

    [HttpGet("info/{id}")]
    [Authorize]
    public async Task<IActionResult> GetCustomerAsync(int id)
    {
        var response = await _webAuthService.GetCustomerAsync(id);

        return Ok(response);
    }

    [HttpPost("info")]
    [Authorize]
    public async Task<IActionResult> UpdateInfo([FromBody] WebCustomerUpdateModel model)
    {
        await _webAuthService.UpdateCustomerAsync(model);

        return Ok();
    }

    [HttpPost("change-pass-word/{id}")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(int id, string password)
    {
        await _webAuthService.ChangePassWordCustomerAsync(id, password);

        return Ok();
    }
}