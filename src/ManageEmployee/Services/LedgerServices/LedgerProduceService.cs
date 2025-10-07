using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.LedgerServices;

public class LedgerProduceService : ILedgerProduceService
{
    private readonly ApplicationDbContext _context;
    private readonly ILedgerProcedureProductService _ledgerProduceExportService;

    public LedgerProduceService(ApplicationDbContext context, ILedgerProcedureProductService ledgerProduceExportService)
    {
        _context = context;
        _ledgerProduceExportService = ledgerProduceExportService;
    }

    public async Task AddProduce(LedgerProduceModel request, int userId, int year)
    {
        var orginalVoucherNumbers = await _context.Ledgers.Where(x => request.LedgerIds.Contains(x.Id)).Select(x => x.OrginalVoucherNumber).ToListAsync();
        var isInternal = await _context.Ledgers.Where(x => request.LedgerIds.Contains(x.Id)).Select(x => x.IsInternal).FirstOrDefaultAsync();
        var ledgers = await _context.Ledgers.Where(x => orginalVoucherNumbers.Contains(x.OrginalVoucherNumber) && x.IsInternal == isInternal).ToListAsync();
        await _ledgerProduceExportService.Create(ledgers, userId, request.Type, year);
    }
}