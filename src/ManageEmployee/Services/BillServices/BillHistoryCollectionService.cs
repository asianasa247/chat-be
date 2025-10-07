using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices;

public class BillHistoryCollectionService : IBillHistoryCollectionService
{
    private readonly ApplicationDbContext _context;
    public BillHistoryCollectionService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<BillHistoryCollection>> GetBillHistoryCollectionForBill(int billId)
    {
        return await _context.BillHistoryCollections.Where(x => x.BillId == billId).ToListAsync();
    }
    public async Task<string> Create(BillHistoryCollection request)
    {
        _context.BillHistoryCollections.Add(request);
        await _context.SaveChangesAsync();
        return string.Empty;
    }
    public async Task<string> Update(BillHistoryCollection request)
    {
        var item = _context.BillHistoryCollections.Find(request.Id);
        item.StatusUserId = request.StatusUserId;
        item.StatusAccountantId = request.StatusAccountantId;
        _context.BillHistoryCollections.Update(item);
        await _context.SaveChangesAsync();
        return string.Empty;
    }
}
