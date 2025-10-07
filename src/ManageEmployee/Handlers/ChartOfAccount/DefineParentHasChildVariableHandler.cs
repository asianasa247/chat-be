using ManageEmployee.Dal.DbContexts;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class DefineParentHasChildVariableHandler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    private readonly bool _hasChild;
    public DefineParentHasChildVariableHandler(ApplicationDbContext context, bool hasChild)
    {
        _context = context;
        _hasChild = hasChild;
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

        if (parentAccount != null && parentAccount.Type <= 5)
            parentAccount.HasChild = _hasChild;
    }
}