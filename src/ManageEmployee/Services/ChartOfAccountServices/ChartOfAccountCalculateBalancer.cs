using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChartOfAccountServices;
public class ChartOfAccountCalculateBalancer: IChartOfAccountCalculateBalancer
{
    private readonly ApplicationDbContext _context;
    public ChartOfAccountCalculateBalancer(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CalculateBalance(ChartOfAccount account, int year, ChartOfAccount parent = null)
    {
        if (parent == null)
        {
            if (account.Type == 6)
            {
                var segments = account.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
                var parentRef = segments[1];
                var grandParentRef = segments[0];
                parent = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x =>
                    x.Code == parentRef && x.ParentRef == grandParentRef && x.WarehouseCode == account.WarehouseCode);

                parent ??= await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x =>
                    x.Code == parentRef && x.ParentRef == grandParentRef);
            }
            else
            {
                parent = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == account.ParentRef && x.WarehouseCode == account.WarehouseCode);
                parent ??= await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == account.ParentRef);
            }
        }

        if (parent != null)
        {
            var isCheckChildrent = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == account.ParentRef).AnyAsync();
            if (!isCheckChildrent)
            {
                parent.HasDetails = false;
                parent.HasChild = false;
                parent.DisplayDelete = true;
                parent.DisplayInsert = true;
            }
            var sumModel = await GetTotal(account, parent, year);
            parent.OpeningDebit = sumModel.OpeningDebit;
            parent.OpeningCredit = sumModel.OpeningCredit;
            parent.OpeningForeignDebit = sumModel.OpeningForeignDebit;
            parent.OpeningForeignCredit = sumModel.OpeningForeignCredit;

            parent.OpeningDebitNB = sumModel.OpeningDebitNB;
            parent.OpeningCreditNB = sumModel.OpeningCreditNB;
            parent.OpeningForeignDebitNB = sumModel.OpeningForeignDebitNB;
            parent.OpeningForeignCreditNB = sumModel.OpeningForeignCreditNB;

            _context.ChartOfAccounts.Update(parent);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(parent.ParentRef))
                await CalculateBalance(parent, year);
        }
    }
    private async Task<SumModel> GetTotal(ChartOfAccount model, ChartOfAccount parent, int year)
    {
        var result = new SumModel()
        {
            OpeningCredit = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode))).SumAsync(x => x.OpeningCredit),
            OpeningDebit = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode))).SumAsync(x => x.OpeningDebit),
            OpeningForeignCredit = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode)))
                .SumAsync(x => x.OpeningForeignCredit),
            OpeningForeignDebit = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode)))
                .SumAsync(x => x.OpeningForeignDebit),
            ClosingCredit = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode)))
                .SumAsync(x => x.OpeningCredit + x.ArisingCredit),
            ClosingDebit = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode)))
                .SumAsync(x => x.OpeningForeignDebit + x.ArisingForeignDebit),
            OpeningCreditNB = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode))).SumAsync(x => x.OpeningCreditNB),
            OpeningDebitNB = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode))).SumAsync(x => x.OpeningDebitNB),
            OpeningForeignCreditNB = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode)))
                .SumAsync(x => x.OpeningForeignCreditNB),
            OpeningForeignDebitNB = await _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef
                    && (coa.WarehouseCode == parent.WarehouseCode || string.IsNullOrEmpty(parent.WarehouseCode)))
                .SumAsync(x => x.OpeningForeignDebitNB),
        };

        return result;
    }

}
