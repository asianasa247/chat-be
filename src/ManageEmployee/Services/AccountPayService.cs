using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Accounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class AccountPayService : IAccountPayService
{
    private readonly ApplicationDbContext _context;
    public AccountPayService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<AccountPay>> GetAll()
    {
        return await _context.AccountPays.ToListAsync();
    }
    public async Task<AccountPay> UpdateAccountPay(AccountPay requests)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var item = _context.AccountPays.Find(requests.Id);
            if (item == null)
            {
                return null;
            }
            item.Name = requests.Name;
            item.Account = requests.Account;
            item.AccountName = requests.AccountName;
            item.Detail1 = requests.Detail1;
            item.DetailName1 = requests.DetailName1;
            item.Detail2 = requests.Detail2;
            item.DetailName2 = requests.DetailName2;

            _context.AccountPays.Update(item);

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return item;
        }
        catch(Exception ex)
        {
            _context.Database.RollbackTransaction();
            throw new ErrorException(ex.Message);
        }
    }
}
