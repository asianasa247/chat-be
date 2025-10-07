using ManageEmployee.Dal.DbContexts;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class TransferDetailFromParentToChildAccountHandler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    public TransferDetailFromParentToChildAccountHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        account.DisplayDelete = true;
        account.DisplayInsert = true;
        account.HasDetails = true;

        var parentCode = account.Code.Substring(0, account.Code.Length - 1);
        var details = _context.GetChartOfAccount(year).Where(x => x.Type >= 5 && x.ParentRef == parentCode).ToList();
        var parentAccount = _context.GetChartOfAccount(year).SingleOrDefault(x => x.Code == parentCode);
        if (parentAccount != null)
        {
            parentAccount.DisplayDelete = false;
            parentAccount.DisplayInsert = false;
            parentAccount.HasDetails = false;
            account.OpeningCredit = 0;
            account.OpeningDebit = 0;
            account.OpeningForeignCredit = 0;
            account.OpeningForeignDebit = 0;


            details.ForEach(item =>
            {
                account.OpeningCredit += item.OpeningCredit;
                account.OpeningDebit += item.OpeningDebit;
                account.OpeningForeignCredit += item.OpeningForeignCredit;
                account.OpeningForeignDebit += item.OpeningForeignDebit;


                item.ParentRef = account.Code;
                var type6ParentRef = $"{parentAccount.Code}:{item.Code}";
                if (item.Type == 5 && _context.GetChartOfAccount(year).Any(x => x.ParentRef == type6ParentRef))
                {
                    var type6Details = _context.GetChartOfAccount(year).Where(x => x.ParentRef == type6ParentRef).ToList();
                    type6Details.ForEach(iItem => { iItem.ParentRef = $"{account.Code}:{item.Code}"; });
                }
            });
        }

        _context.SaveChanges();
    }
}