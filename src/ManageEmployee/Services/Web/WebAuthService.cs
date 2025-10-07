using Common.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.Webs;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.DataTransferObject;
using NuGet.Protocol;

namespace ManageEmployee.Services.Web;
public class WebAuthService : IWebAuthService
{
    private readonly ICustomerService _customerService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public WebAuthService(ApplicationDbContext context, IConfiguration configuration, ICustomerService customerService)
    {
        _context = context;
        _configuration = configuration;
        _customerService = customerService;
    }
    public async Task<Customer> Authenticate(string username, string password)
    {

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(username))
        {
            throw new ErrorException("Tài khoản đăng nhập không đúng hoặc không tồn tại!");
        }
        var customer = await _context.Customers.Where(x => x.Phone != null && x.Phone.Equals(username.Trim())
                                                  && MD5Ultility.HashPassword(password, MD5Ultility.saltKey).Equals(x.Password)).FirstOrDefaultAsync();
        if (customer is null)
        {
            throw new ErrorException("Tài khoản đăng nhập không đúng hoặc không tồn tại!");
        }
        return customer;
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<Customer> Register(WebCustomerV2Model model)
    {
        if (string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Password) ||
            string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Password) ||
            model.Phone.Length < 10)
        {
            throw new ErrorException(ErrorMessage.PHONE_NUMBER_IS_NOT_FORMAT);
        }
        var checkCustomer = await _context.Customers.Where(x => x.Phone != null && x.Phone.Equals(model.Phone.Trim())).FirstOrDefaultAsync();
        if (checkCustomer != null)
        {
            throw new ErrorException(ErrorMessage.PHONE_NUMBER_IS_EXIST);
        }

        var customer = new Customer
        {
            Phone = model.Phone,
            Name = model.Name,
            WardId = model.WardId,
            ProvinceId = model.ProvinceId,
            DistrictId = model.DistrictId,
            Birthday = model.Birthday,
            Address = model.Address,
            Email = model.Email,
            UserCreated = null,
            Gender = model.Gender,
            Password = MD5Ultility.HashPassword(model.Password, MD5Ultility.saltKey)
        };
        customer.Code = await _customerService.GetCodeCustomer(0);
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    /// <summary>
    /// Update email
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task UpdateMail(WebCustomerV2Model model)
    {
        var customer = await _context.Customers.Where(cus => cus.Id == model.Id).FirstOrDefaultAsync();
        if (customer != null)
        {
            customer.Email = model.Email;
            customer.SendEmail = true;
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }
    }

    /// <summary>
    /// Tạo token đăng nhập
    /// </summary>
    /// <param name="loginResult"></param>
    /// <returns></returns>
    public string GenerateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }

    public async Task<Customer> RegisterAccountSocial(AuthenticateSocialModel model)
    {
        Console.WriteLine("model: " + model.ProviderId);

        var customer = new Customer
        {
            Phone = model.Phone,
            Name = model.Name,
            Birthday = model.Birthday,
            Address = model.Address,
            Email = model.Email,
            UserCreated = null,
            Gender = model.Gender,
            Avatar =model.PhotoUrl,
            Provider = model.Provider,
            ProviderId = model.ProviderId.ToString(),
            Type = 2
        };
        customer.Code = await _customerService.GetCodeCustomer(0);
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateCustomerAsync(WebCustomerUpdateModel form)
    {
        var customer = await _context.Customers.FindAsync(form.Id);
        if (customer is null)
        {
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
        }

        customer.Email = form.Email;
        customer.Gender = form.Gender;
        customer.Birthday = form.Birthday;
        customer.Name = form.Name;
        customer.Avatar = form.Avatar;
        customer.Address = form.Address;
        customer.Phone = form.Phone;
        customer.ProvinceId = form.ProvinceId;
        customer.DistrictId = form.DistrictId;
        customer.WardId = form.WardId;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
    public async Task ChangePassWordCustomerAsync(int id, string password)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
        }
        customer.Password = MD5Ultility.HashPassword(password, MD5Ultility.saltKey);
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
    public async Task<WebCustomerUpdateModel> GetCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is null)
        {
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
        }

        return new WebCustomerUpdateModel
        {
            Id = customer.Id,
            Address = customer.Address,
            Avatar = customer.Avatar,
            Birthday = customer.Birthday,
            DistrictId = customer.DistrictId,
            Email = customer.Email,
            Gender = customer.Gender,
            Name = customer.Name,
            Phone = customer.Phone,
            ProvinceId = customer.ProvinceId,
            WardId = customer.WardId
        };
    }
}
