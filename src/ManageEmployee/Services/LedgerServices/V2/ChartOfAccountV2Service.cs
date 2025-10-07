using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.LedgerServices.V2;
public class ChartOfAccountV2Service : IChartOfAccountV2Service
{
    private readonly ApplicationDbContext _context;
    public ChartOfAccountV2Service(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CommonModel> FindAccount(string code, string parentRef, int year)
    {
        return await _context.GetChartOfAccount(year)
            .Where(x => x.Code == code && (string.IsNullOrEmpty(parentRef) || x.ParentRef == parentRef))
            .Select(account => new CommonModel
            {
                Id = account.Id,
                Code = account.Code,
                Name = account.Name,
                IsForeignCurrency = account.IsForeignCurrency,
                AccGroup = account.AccGroup,
                Classification = account.Classification,
                HasDetails = account.HasDetails,
                WarehouseCode = account.WarehouseCode,
                DisplayInsert = account.DisplayInsert,
                Duration = account.Duration,
                Protected = account.Protected,
                ParentRef = account.ParentRef,

            }).FirstOrDefaultAsync();
    }
}
