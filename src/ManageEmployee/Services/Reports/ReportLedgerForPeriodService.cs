using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Reports;
public class ReportLedgerForPeriodService : IReportLedgerForPeriodService
{
    private readonly ApplicationDbContext _context;

    public ReportLedgerForPeriodService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task PerformAsync(LedgerReportParam param)
    {
        if(param.FilterType == 1)
        {
            param.FromDate = new DateTime(DateTime.Now.Year, param.FromMonth ?? 0, 1);
            param.ToDate = new DateTime(DateTime.Now.Year, param.ToMonth ?? 0, 1).AddMonths(1);
        }
        else
        {
            param.ToDate = param.ToDate.Value.AddDays(1);
        }
        var ledger = await _context.GetLedgerNotForYear(param.IsNoiBo ? 3: 2).Where(x => x.OrginalBookDate >= param.FromDate && x.OrginalBookDate < param.ToDate).ToListAsync();

    }
}
