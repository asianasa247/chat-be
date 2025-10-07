using ManageEmployee.Dal.DbContexts;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class ParentCrudActionPermissionHandler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    private readonly bool _allowInsert;
    private readonly bool _allowDelete;

    public ParentCrudActionPermissionHandler(ApplicationDbContext context, bool allowInsert = true, bool allowUpdate = true,
        bool allowDelete = true)
    {
        _context = context;
        _allowInsert = allowInsert;
        _allowDelete = allowDelete;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        Entities.ChartOfAccountEntities.ChartOfAccount parentAccount;
        if (account.Type == 6)
        {
            var segments = account.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var parentRef = segments[1];
            var grandParentRef = segments[0];
            parentAccount =
                _context.GetChartOfAccount(year).SingleOrDefault(x =>
                    x.Code == parentRef && x.ParentRef == grandParentRef &&
                    x.WarehouseCode == account.WarehouseCode);
        }
        else
        {
            parentAccount = _context.GetChartOfAccount(year).SingleOrDefault(x => x.Code == account.ParentRef);
        }

        if (parentAccount != null)
        {
            parentAccount.DisplayDelete = _allowDelete;
            parentAccount.DisplayInsert = _allowInsert;
        }
    }
}