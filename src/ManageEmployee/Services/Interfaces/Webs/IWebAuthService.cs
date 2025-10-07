using System.Security.Claims;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.Services.Interfaces.Webs;

/// <summary>
/// Interface
/// </summary>
public interface IWebAuthService
{
    Task<Customer> Authenticate(string username, string password);
    Task ChangePassWordCustomerAsync(int id, string password);
    string GenerateToken(List<Claim> authClaims);
    Task<WebCustomerUpdateModel> GetCustomerAsync(int id);
    Task<Customer> Register(WebCustomerV2Model model);
    Task<Customer> RegisterAccountSocial (AuthenticateSocialModel model);
    Task UpdateCustomerAsync(WebCustomerUpdateModel form);
    Task UpdateMail(WebCustomerV2Model model);
}
