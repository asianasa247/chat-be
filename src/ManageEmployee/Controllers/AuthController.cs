using AutoMapper;
using ManageEmployee.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Services.Interfaces.Menus;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.Constants;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.RegisterModels;
using ManageEmployee.Services.Interfaces.Invoices;
using ManageEmployee.Extends;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IUserRoleService _userRoleService;
    private readonly ApplicationDbContext _context;
    private readonly IMenuService _menuService;
    private readonly IVintaxOneInvoiceCreator _vintaxOneInvoiceCreator;

    public AuthController(
        IUserService userService,
        IMapper mapper,
        IConfiguration configuration,
        IUserRoleService userRoleService, ApplicationDbContext context, IMenuService menuService, IVintaxOneInvoiceCreator vintaxOneInvoiceCreator)
    {
        _userService = userService;
        _mapper = mapper;
        _configuration = configuration;
        _userRoleService = userRoleService;
        _context = context;
        _menuService = menuService;
        _vintaxOneInvoiceCreator = vintaxOneInvoiceCreator;
    }

    [AllowAnonymous]
    [HttpPost("requestForgotPass")]
    public async Task<IActionResult> RequestForgotPass([FromBody] ResetPasswordModel model)
    {
        if (!String.IsNullOrEmpty(model.Username))
        {
            var user = await _userService.GetByUserName(model.Username);
            if (user != null)
            {
                user.RequestPassword = true;
                var data = _mapper.Map<UserModel>(user);
                await _userService.Update(data);
                return Ok(true);
            }
        }

        return Ok(false);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticateModel model)
    {
        try
        {
            var userName = model.Username?.Trim();
            var password = model.Password?.Trim(); 

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Ok(new ObjectReturn
                {
                    message = ResultErrorConstants.USER_IS_NOT_EXIST,
                    status = Convert.ToInt32(ErrorEnum.USER_IS_NOT_EXIST)
                });
            }

            var user = await _userService.GetByUserName(userName);
            if (user == null)
            {
                return Ok(new ObjectReturn
                {
                    message = ResultErrorConstants.USER_IS_NOT_EXIST,
                    status = Convert.ToInt32(ErrorEnum.USER_IS_NOT_EXIST)
                });
            }

            UserMapper.Auth checkUser = await _userService.Authenticate(userName, password);
            if (checkUser == null)
            {
                return Ok(new ObjectReturn
                {
                    message = ResultErrorConstants.ERROR_PASS,
                    status = Convert.ToInt32(ErrorEnum.ERROR_PASS)
                });
            }

            await _userService.UpdateLastLogin(user.Id);

            List<string> listRole = user.UserRoleIds.Split(",").ToList();
            var userRoles = await _userRoleService.GetAll_Login();
            var roles = userRoles.Where(o => listRole.Contains(o.Id.ToString())).Select(x => x.Code).ToList();

            var tokenHandler = new JwtSecurityTokenHandler();
            var authClaims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("FullName",  !String.IsNullOrEmpty(user.FullName)? user.FullName : ""),
                    new Claim("RoleName", JsonConvert.SerializeObject(roles)),
                };

            var token = GetToken(authClaims);

            var tokenString = tokenHandler.WriteToken(token);
            var color = await _context.MainColors.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var configurationViews = await _context.ConfigurationViews.ToListAsync();

            var listMenuCheckRole = await _menuService.GetListMenu(user.Id);
            // return basic user info and authentication token

            return Ok(new ObjectReturn
            {
                data = new
                {
                    Id = user.Id,
                    Username = user.Username,
                    Fullname = user.FullName,
                    Avatar = user.Avatar,
                    Timekeeper = user.Timekeeper,
                    Token = tokenString,
                    TargetId = user.TargetId,
                    RoleName = roles,
                    Menus = listMenuCheckRole,
                    UserRoleIds = user.UserRoleIds,
                    IsDark = color?.IsDark ?? false,
                    Theme = color?.Theme,
                    YearCurrent = user.YearCurrent,
                    ConfigurationViewTypePay = JsonConvert.DeserializeObject<List<SelectTypePayModel>>(configurationViews.Find(X => X.FieldName == "TypePay")?.Value),
                    ConfigurationViewQuantityBoxNec = configurationViews.Find(X => X.FieldName == "QuantityBoxNec")?.Value,
                    ConfigurationViewPrint = configurationViews.Find(X => X.FieldName == "PrintBill")?.Value,
                    ConfigurationViewLayout = configurationViews.Find(X => X.FieldName == "Layout")?.Value,
                },
                status = 200,
                message = ResultErrorConstants.LOGIN_SUCCESS,
            });
        }
        catch (Exception ex)
        {
            return Ok(new ObjectReturn
            {
                message = ex.Message,
                status = 400
            });
        }
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(8),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // map model to entity
        var user = _mapper.Map<UserModel>(model);
        // create user
        await _userService.Create(user, model.Password);
        return Ok();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordModel model)
    {
        if (model.Id == 0)
        {
            model.Id = HttpContext.GetIdentityUser().Id;
        }

        // check is current user
        if (!await _userService.CheckPassword(model.Id, model.OldPassword))
        {
            return Ok(new ObjectReturn
            {
                message = ErrorEnum.ERROR_PASS.ToString(),
                status = Convert.ToInt32(ErrorEnum.ERROR_PASS)
            });
        }

        await _userService.UpdatePassword(model);
        return Ok(new ObjectReturn
        {
            data = ErrorEnum.SUCCESS,
            status = Convert.ToInt32(ErrorEnum.SUCCESS)
        });
    }

    [AllowAnonymous]
    [HttpPost("guess-login")]
    public async Task<IActionResult> GuessLogin()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var authClaims = new List<Claim>
                {
                    new("UserId", "0"),
                    new(ClaimTypes.Name, "guess"),
                    new("FullName",  "Guess"),
                    new("RoleName", "[]"),
                };

        var token = GetToken(authClaims);

        var tokenString = tokenHandler.WriteToken(token);
        var configurationViews = await _context.ConfigurationViews.ToListAsync();

        // return basic user info and authentication token
        return Ok(
                new ObjectReturn
                {
                    data = new
                    {
                        Id = 0,
                        Username = "Guess",
                        Fullname = "Guess",
                        Avatar = "",
                        Timekeeper = "",
                        Token = tokenString,
                        TargetId = "",
                        RoleName = "[]",
                        Menus = "[]",
                        UserRoleIds = "",
                        IsDark = "",
                        Theme = "",
                        YearCurrent = DateTime.Now.Year,
                        ConfigurationViewTypePay = JsonConvert.DeserializeObject<List<SelectTypePayModel>>(configurationViews.Find(X => X.FieldName == "TypePay")?.Value),
                        ConfigurationViewQuantityBoxNec = configurationViews.Find(X => X.FieldName == "QuantityBoxNec")?.Value,
                        ConfigurationViewPrint = configurationViews.Find(X => X.FieldName == "PrintBill")?.Value,
                        ConfigurationViewLayout = configurationViews.Find(X => X.FieldName == "Layout")?.Value
                    },
                    status = 200,
                    message = ResultErrorConstants.LOGIN_SUCCESS,
                });
    }


    [AllowAnonymous]
    [HttpPost("invoice")]
    public async Task<IActionResult> CreateInvoice(int billId)
    {
        await _vintaxOneInvoiceCreator.CreateInvoice(billId);
        return Ok(true);
    }
}