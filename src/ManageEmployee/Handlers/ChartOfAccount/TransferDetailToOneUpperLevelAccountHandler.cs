using ManageEmployee.Dal.DbContexts;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class TransferDetailToOneUpperLevelAccountHandler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    public TransferDetailToOneUpperLevelAccountHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        var parentCode = account.Code.Substring(0, account.Code.Length - 1);
        var details = _context.GetChartOfAccount(year).Where(x => x.ParentRef == account.Code).ToList();
        var upperLevelAccount = _context.GetChartOfAccount(year)
            .Where(x => x.Code == parentCode).OrderByDescending(x => x.Code)
            .FirstOrDefault();
        if (upperLevelAccount != null)
        {
            upperLevelAccount.HasDetails = true;
            details.ForEach(item => item.ParentRef = upperLevelAccount.Code);
        }

        _context.SaveChanges();
    }
}