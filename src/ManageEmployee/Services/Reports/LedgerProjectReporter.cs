using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Reports
{
    public class LedgerProjectReporter: ILedgerProjectReporter
    {
        private readonly ApplicationDbContext _context;
        public LedgerProjectReporter(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LedgerProjectReporterModel>> ReportAsync(DateTime fromAt, DateTime toAt, string projectCode, int year, bool isNoiBo)
        {
            return await _context.GetLedger(year, isNoiBo ? 3 : 1)
                .Where(x => x.ProjectCode == projectCode && x.OrginalBookDate >= fromAt && x.OrginalBookDate <= toAt)
                .Select(x => new LedgerProjectReporterModel
                {
                    ProjectCode = x.ProjectCode,
                    OrginalBookDate = x.OrginalBookDate,
                    Type = x.Type,
                    Amount = x.Amount,
                    BookDate = x.BookDate,
                    VoucherNumber = x.VoucherNumber,
                    CreditCode = x.CreditCode,
                    CreditDetailCodeFirst = x.CreditDetailCodeFirst,
                    CreditDetailCodeSecond = x.CreditDetailCodeSecond,
                    DebitCode  = x.DebitCode,
                    DebitDetailCodeFirst = x.DebitDetailCodeFirst,
                    DebitDetailCodeSecond = x.DebitDetailCodeSecond,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    CreditWarehouse = x.CreditWarehouse,
                    DebitWarehouse = x.DebitWarehouse,
                    Month = x.Month,
                    OrginalDescription = x.OrginalDescription,
                    CreditCodeName = x.CreditCodeName,
                    CreditDetailCodeFirstName = x.CreditDetailCodeFirstName,
                    CreditDetailCodeSecondName = x.CreditDetailCodeSecondName,
                    DebitCodeName = x.DebitCodeName,
                    DebitDetailCodeFirstName = x.DebitDetailCodeFirstName,
                    DebitDetailCodeSecondName= x.DebitDetailCodeSecondName,
                    CreditWarehouseName = x.CreditWarehouseName,
                    DebitWarehouseName = x.DebitWarehouseName,
                }).ToListAsync();
        }
    }
}