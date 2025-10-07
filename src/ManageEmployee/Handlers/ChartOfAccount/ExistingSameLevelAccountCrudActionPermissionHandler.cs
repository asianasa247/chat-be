using ManageEmployee.Dal.DbContexts;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class ExistingSameLevelAccountCrudActionPermissionHandler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    public ExistingSameLevelAccountCrudActionPermissionHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        account.DisplayInsert = true;
        account.DisplayDelete = true;

        //var otherAccounts = _context.ChartOfAccounts.Where(x => x.Type == account.Type).ToList();
        //otherAccounts.ForEach(item => item.DisplayDelete = false);
        var parentAccount =
            _context.GetChartOfAccount(year).SingleOrDefault(x =>
                x.Code == account.Code.Substring(0, account.Code.Length - 1));
        if (parentAccount != null)
        {
            parentAccount.DisplayDelete = false;
            parentAccount.DisplayInsert = false;
        }
    }
}