using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.Interfaces.Accounts;

public interface IAccountPayService
{
    Task<IEnumerable<AccountPay>> GetAll();
    Task<AccountPay> UpdateAccountPay(AccountPay requests);
}
