using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Queues;
public class LedgerImportErrorQueue: ILedgerImportErrorQueue
{
    private readonly ApplicationDbContext _context;
    public LedgerImportErrorQueue(ApplicationDbContext context)
    {
        _context = context;
    }
    public void Perform(List<Ledger> dataImports, int year)
    {
        var debitCodes = dataImports.Select(x => x.DebitCode);
        var debitCodeFirsts = dataImports.Select(x => x.DebitDetailCodeFirst);
        var debitCodeSeconds = dataImports.Select(x => x.DebitDetailCodeSecond);
        var creditCodes = dataImports.Select(x => x.CreditCode);
        var creditCodeFirsts = dataImports.Select(x => x.CreditDetailCodeFirst);
        var creditCodeSeconds = dataImports.Select(x => x.CreditDetailCodeSecond);
        var accountCodes = debitCodes.Concat(debitCodeFirsts).Concat(debitCodeSeconds)
            .Concat(creditCodes).Concat(creditCodeFirsts).Concat(creditCodeSeconds)
            .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
        var accounts = _context.GetChartOfAccount(year).Where(x => accountCodes.Contains(x.Code)).ToList();
        var dataAdds = new List<LedgerErrorImport>();

        foreach (var dataImport in dataImports)
        {
            var ledgerError = new LedgerErrorImport()
            {
                LedgerId = dataImport.Id,
                IsInternal = dataImport.IsInternal,
            };
            var debit = accounts.Find(x => x.Code == dataImport.DebitCode);
            if(debit is null)
            {
                ledgerError.ErrorCodes += "DebitCode;";
            }

            if(!string.IsNullOrEmpty(dataImport.DebitDetailCodeFirst))
            {
                var debitFirst = accounts.Find(x => x.Code == dataImport.DebitDetailCodeFirst && x.ParentRef == dataImport.DebitCode);
                if (debitFirst is null)
                {
                    ledgerError.ErrorCodes += "DebitDetailCodeFirst;";
                }
            }

            if (!string.IsNullOrEmpty(dataImport.DebitDetailCodeSecond))
            {
                var debitSecond = accounts.Find(x => x.Code == dataImport.DebitDetailCodeSecond && x.ParentRef == (dataImport.DebitCode + ":" + dataImport.DebitDetailCodeFirst));
                if (debitSecond is null)
                {
                    ledgerError.ErrorCodes += "DebitDetailCodeSecond;";
                }
            }

            var credit = accounts.Find(x => x.Code == dataImport.CreditCode);
            if (credit is null)
            {
                ledgerError.ErrorCodes += "CreditCode;";
            }

            if (!string.IsNullOrEmpty(dataImport.CreditDetailCodeFirst))
            {
                var creditFirst = accounts.Find(x => x.Code == dataImport.CreditDetailCodeFirst && x.ParentRef == dataImport.CreditCode);
                if (creditFirst is null)
                {
                    ledgerError.ErrorCodes += "CreditDetailCodeFirst;";
                }
            }

            if (!string.IsNullOrEmpty(dataImport.CreditDetailCodeSecond))
            {
                var creditSecond = accounts.Find(x => x.Code == dataImport.CreditDetailCodeSecond && x.ParentRef == (dataImport.CreditCode + ":" + dataImport.CreditDetailCodeFirst));
                if (creditSecond is null)
                {
                    ledgerError.ErrorCodes += "CreditDetailCodeSecond;";
                }
            }

            if (!string.IsNullOrEmpty(ledgerError.ErrorCodes))
            {
                dataAdds.Add(ledgerError);
            }
        }
        _context.LedgerErrorImports.AddRange(dataAdds);
        _context.SaveChanges();
    }
}
