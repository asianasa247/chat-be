using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Ledgers;

namespace ManageEmployee.Services.LedgerServices;
public class LedgerUpdateChartOfAccountNameService : ILedgerUpdateChartOfAccountNameService
{
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ApplicationDbContext _context;

    public LedgerUpdateChartOfAccountNameService(IChartOfAccountService chartOfAccountService, ApplicationDbContext context)
    {
        _chartOfAccountService = chartOfAccountService;
        _context = context;
    }
    public async Task<Ledger> UpdateChartOfAccountName(Ledger cEntity, int year)
    {
        var coaDebit = await _chartOfAccountService.GetAccountByCode(cEntity.DebitCode, year);
        var coaDebitDetail1 = await _chartOfAccountService.GetAccountByCode(cEntity.DebitDetailCodeFirst, year, cEntity.DebitCode, cEntity.DebitWarehouse);
        var coaDebitDetail2 = await _chartOfAccountService.GetAccountByCode(cEntity.DebitDetailCodeSecond, year,
                                                    string.IsNullOrEmpty(cEntity.DebitDetailCodeFirst) ? "" : cEntity.DebitCode + ":" + cEntity.DebitDetailCodeFirst, cEntity.DebitWarehouse);

        var coaCredit = await _chartOfAccountService.GetAccountByCode(cEntity.CreditCode, year);
        var coaCreditDetail1 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeFirst, year, cEntity.CreditCode, cEntity.CreditWarehouse);
        var coaCreditDetail2 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeSecond, year,
                                string.IsNullOrEmpty(cEntity.CreditDetailCodeFirst) ? "" : cEntity.CreditCode + ":" + cEntity.CreditDetailCodeFirst, cEntity.CreditWarehouse);

        cEntity.DebitCodeName = coaDebit?.Name;
        cEntity.DebitDetailCodeFirstName = coaDebitDetail1?.Name;
        cEntity.DebitDetailCodeSecondName = coaDebitDetail2?.Name;
        cEntity.CreditCodeName = coaCredit?.Name;
        cEntity.CreditDetailCodeFirstName = coaCreditDetail1?.Name;
        cEntity.CreditDetailCodeSecondName = coaCreditDetail2?.Name;

        cEntity.DebitCode = cEntity.DebitCode ?? string.Empty;
        cEntity.DebitDetailCodeFirst = cEntity.DebitDetailCodeFirst ?? string.Empty;
        cEntity.DebitDetailCodeSecond = cEntity.DebitDetailCodeSecond ?? string.Empty;
        cEntity.CreditCode = cEntity.CreditCode ?? string.Empty;
        cEntity.CreditDetailCodeFirst = cEntity.CreditDetailCodeFirst ?? string.Empty;
        cEntity.CreditDetailCodeSecond = cEntity.CreditDetailCodeSecond ?? string.Empty;

        cEntity.DebitCodeName = cEntity.DebitCodeName ?? string.Empty;
        cEntity.DebitDetailCodeFirstName = cEntity.DebitDetailCodeFirstName ?? string.Empty;
        cEntity.DebitDetailCodeSecondName = cEntity.DebitDetailCodeSecondName ?? string.Empty;
        cEntity.CreditCodeName = cEntity.CreditCodeName ?? string.Empty;
        cEntity.CreditDetailCodeFirstName = cEntity.CreditDetailCodeFirstName ?? string.Empty;
        cEntity.CreditDetailCodeSecondName = cEntity.CreditDetailCodeSecondName ?? string.Empty;

        if (!string.IsNullOrEmpty(cEntity.DebitWarehouse))
        {
            cEntity.DebitWarehouse = coaDebitDetail2?.WarehouseCode ?? coaDebitDetail1?.WarehouseCode ?? coaDebit?.WarehouseCode ?? string.Empty;
            cEntity.DebitWarehouseName = _context.Warehouses.FirstOrDefault(t => t.Code == cEntity.DebitWarehouse)?.Name;
        }
        if (!string.IsNullOrEmpty(cEntity.CreditWarehouse))
        {
            cEntity.CreditWarehouse = coaCreditDetail2?.WarehouseCode ?? coaCreditDetail1?.WarehouseCode ?? coaCredit?.WarehouseCode ?? string.Empty;
            cEntity.CreditWarehouseName = _context.Warehouses.FirstOrDefault(t => t.Code == cEntity.CreditWarehouse)?.Name;
        }

        cEntity.Classification = coaDebitDetail2?.Classification ?? coaDebitDetail1?.Classification;
        return cEntity;
    }

}
