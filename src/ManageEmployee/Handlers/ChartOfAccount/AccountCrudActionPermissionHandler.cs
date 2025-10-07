namespace ManageEmployee.Handlers.ChartOfAccount;

public class AccountCrudActionPermissionHandler : IChartOfAccountHandler
{
    private readonly bool _allowInsert;
    private readonly bool _allowUpdate;
    private readonly bool _allowDelete;

    public AccountCrudActionPermissionHandler(bool allowInsert = true, bool allowUpdate = true,
        bool allowDelete = true)
    {
        _allowInsert = allowInsert;
        _allowUpdate = allowUpdate;
        _allowDelete = allowDelete;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        account.DisplayDelete = _allowDelete;
        account.DisplayInsert = _allowInsert;
    }
}