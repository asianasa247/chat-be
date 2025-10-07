using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ProduceProductServices.PlanningProduceProductServices;
public class PlanningWithLedgerService: IPlanningWithLedgerService
{
    private readonly ApplicationDbContext _context;
    private readonly IPlanningProduceProductService _planningProduceProductService;

    public PlanningWithLedgerService(ApplicationDbContext context, IPlanningProduceProductService planningProduceProductService)
    {
        _context = context;
        _planningProduceProductService = planningProduceProductService;
    }

    public async Task SetDataAsync(PlanningProduceProductModel form, int userId)
    {
        var detailIds = form.Items.Select(x => x.Id);
        var ledgerImportDetails = await _context.LedgerProcedureProductDetails.Where(x => detailIds.Contains(x.Id)).ToListAsync();

        var planningSetForm = new PlanningProduceProductModel
        {
            ProcedureCode = ProcedureForCreatePlanningProduct.LEDGER_IMPORT,
            Note = form.Note,
            Date = form.Date,
            Items = new List<PlanningProduceProductDetailModel>(),
            Id = form.Id,
        };

        foreach (var ledgerImportDetail in ledgerImportDetails)
        {
            var detail = new PlanningProduceProductDetailModel
            {
                Quantity = ledgerImportDetail.Quantity,
                UnitPrice = ledgerImportDetail.UnitPrice,
            };
            detail.GoodsId = await _context.Goods
                .Where(x => x.Account == ledgerImportDetail.DebitCode && x.Detail1 == ledgerImportDetail.DebitDetailCodeFirst
                                  && (string.IsNullOrEmpty(ledgerImportDetail.DebitDetailCodeSecond) || x.Detail2 == ledgerImportDetail.DebitDetailCodeSecond))
                .Select(x => x.Id).FirstOrDefaultAsync();
            detail.CustomerId = await _context.Customers.Where(X => X.DebitCode == ledgerImportDetail.CreditCode && X.DebitDetailCodeFirst == ledgerImportDetail.CreditDetailCodeFirst
                                    && (string.IsNullOrEmpty(ledgerImportDetail.CreditDetailCodeSecond) || X.DebitDetailCodeSecond == ledgerImportDetail.CreditDetailCodeSecond))
                .Select(x => x.Id).FirstOrDefaultAsync();

            planningSetForm.Items.Add(detail);
        }
        if (form.Id > 0)
        {
            await _planningProduceProductService.Update(planningSetForm, userId);
        }
        else
        {
            await _planningProduceProductService.Create(planningSetForm, userId);
        }
    }
}
