using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ManageEmployee.Queues;

public class ChartOfAccountCaculatorQueue : IChartOfAccountCaculatorQueue
{
    private readonly IDbContextFactory _dbContextFactory;
    private readonly SettingDatabase _settingDatabase;
    private ApplicationDbContext dbContext;
    private readonly IConnectionStringProvider _connectionStringProvider;
    public ChartOfAccountCaculatorQueue(
        IDbContextFactory dbContextFactory,
        IOptions<SettingDatabase> settingDatabase,
        IConnectionStringProvider connectionStringProvider
    )
    {
        _dbContextFactory = dbContextFactory;
        _settingDatabase = settingDatabase.Value;
        _connectionStringProvider = connectionStringProvider;
    }

    public void Perform(int year, string dbName)
    {
        var connectionStr = _connectionStringProvider.GetConnectionString(dbName);

        dbContext = _dbContextFactory.GetDbContext(connectionStr);
        var queue = dbContext.UpdateArisingAccountQueue.FirstOrDefault(x => x.Status != "SUCCESS");
        if (queue == null)
            return;

        var listAccount = dbContext.GetChartOfAccount(year).ToList();

        var listLedger = dbContext.Ledgers.Where(x => x.Year == year).ToList();
        var accountParents = listAccount.Where(x => string.IsNullOrEmpty(x.ParentRef));
        foreach (var account in accountParents)
        {
            DeQuyUpdateChartOfAccount(listAccount, listLedger, account, year);
        }
        queue.Status = "SUCCESS";
        dbContext.UpdateArisingAccountQueue.Update(queue);

        dbContext.SaveChanges();
    }

    private void DeQuyUpdateChartOfAccount(List<ChartOfAccount> accounts, List<Ledger> listLedger, ChartOfAccount account, int year)
    {
        string parentRef = account.Code;
        if (account.Type == 5)
        {
            parentRef = account.ParentRef + ":" + account.Code;
        }
        var accountChidrens = accounts.Where(x => x.ParentRef == parentRef
        && (x.WarehouseCode == account.WarehouseCode || string.IsNullOrEmpty(x.WarehouseCode))).ToList();

        foreach (var accountChidren in accountChidrens)
        {
            DeQuyUpdateChartOfAccount(accounts, listLedger, accountChidren, year);
        }

        if (accountChidrens.Count > 0)
        {
            account.OpeningDebit = accountChidrens.Sum(x => x.OpeningDebit);
            account.OpeningCredit = accountChidrens.Sum(x => x.OpeningCredit);
            account.OpeningForeignDebit = accountChidrens.Sum(x => x.OpeningForeignDebit);
            account.OpeningForeignCredit = accountChidrens.Sum(x => x.OpeningForeignCredit);

            account.OpeningDebitNB = accountChidrens.Sum(x => x.OpeningDebitNB);
            account.OpeningCreditNB = accountChidrens.Sum(x => x.OpeningCreditNB);
            account.OpeningForeignDebitNB = accountChidrens.Sum(x => x.OpeningForeignDebitNB);
            account.OpeningForeignCreditNB = accountChidrens.Sum(x => x.OpeningForeignCreditNB);
        }
        List<Ledger> ledgerDebits;
        List<Ledger> ledgerCredits;
        if (account.Type == 5)
        {
            ledgerDebits = listLedger.Where(x => x.DebitDetailCodeFirst == account.Code && x.DebitCode == account.ParentRef
                            && (string.IsNullOrEmpty(account.WarehouseCode) || x.DebitWarehouse == account.WarehouseCode)).ToList();
            ledgerCredits = listLedger.Where(x => x.CreditDetailCodeFirst == account.Code && x.CreditCode == account.ParentRef
                            && (string.IsNullOrEmpty(account.WarehouseCode) || x.CreditWarehouse == account.WarehouseCode)).ToList();
        }
        else if (account.Type == 6)
        {
            string[] codeParent = account.ParentRef.Split(':');
            ledgerDebits = listLedger.Where(x => x.DebitDetailCodeSecond == account.Code && x.DebitCode == codeParent[0] && x.DebitDetailCodeFirst == codeParent[1]
                            && (string.IsNullOrEmpty(account.WarehouseCode) || x.DebitWarehouse == account.WarehouseCode)).ToList();
            ledgerCredits = listLedger.Where(x => x.CreditDetailCodeSecond == account.Code && x.CreditCode == codeParent[0] && x.CreditDetailCodeFirst == codeParent[1]
                            && (string.IsNullOrEmpty(account.WarehouseCode) || x.CreditWarehouse == account.WarehouseCode)).ToList();
        }
        else
        {
            ledgerDebits = listLedger.Where(x => x.DebitCode == account.Code).ToList();
            ledgerCredits = listLedger.Where(x => x.CreditCode == account.Code).ToList();
        }

        UpdateChartOfAccount(account, ledgerDebits, ledgerCredits, year);
    }

    private void UpdateChartOfAccount(ChartOfAccount currentAccount, IEnumerable<Ledger> ledgerDebits, IEnumerable<Ledger> ledgerCredits, int year)
    {
        var ledgerDebitInternals = ledgerDebits.Where(x => x.IsInternal == 3);
        var ledgerDebitExternals = ledgerDebits.Where(x => x.IsInternal == 1 || x.IsInternal == 2);

        var ledgerCreditInternals = ledgerCredits.Where(x => x.IsInternal == 3);
        var ledgerCreditExternals = ledgerCredits.Where(x => x.IsInternal == 1 || x.IsInternal == 2);

        // Nếu là ngoại tệ
        if (currentAccount.IsForeignCurrency)
        {
            currentAccount.ArisingForeignDebit = ledgerDebitExternals.Sum(x => x.Amount);
            currentAccount.ArisingForeignDebitNB = ledgerDebitInternals.Sum(x => x.Amount);
            currentAccount.ArisingForeignCredit = ledgerCreditExternals.Sum(x => x.Amount);
            currentAccount.ArisingForeignCreditNB = ledgerCreditInternals.Sum(x => x.Amount);
        }
        else // Không phải ngoại tệ
        {
            currentAccount.ArisingDebit = ledgerDebitExternals.Sum(x => x.Amount);
            currentAccount.ArisingDebitNB = ledgerDebitInternals.Sum(x => x.Amount);
            currentAccount.ArisingCredit = ledgerCreditExternals.Sum(x => x.Amount);
            currentAccount.ArisingCreditNB = ledgerCreditInternals.Sum(x => x.Amount);
        }

        // Tính toán số lượng
        currentAccount.ArisingStockQuantity = ledgerDebitExternals.Sum(x => x.Quantity) - ledgerCreditExternals.Sum(x => x.Quantity);
        currentAccount.ArisingStockQuantityNB = ledgerDebitInternals.Sum(x => x.Quantity) - ledgerCreditInternals.Sum(x => x.Quantity);

        if (currentAccount.Id > 0)
            dbContext.ChartOfAccounts.Update(currentAccount);
        else
        {
            currentAccount.Year = year;

            dbContext.ChartOfAccounts.Add(currentAccount);
        }
    }
}