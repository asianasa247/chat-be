using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices;
public class TillManagerService : ITillManagerService
{
    private readonly ApplicationDbContext _context;

    public TillManagerService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PagingResult<TillManager>> GetAll(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 0;

            var datas = _context.TillManagers;

            var result = new PagingResult<TillManager>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Data = await datas.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                TotalItems = await datas.CountAsync()
            };
            return result;
        }
        catch
        {
            return new PagingResult<TillManager>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<TillManager>()
            };
        }
    }

    public async Task<TillManager> Create(TillManager request)
    {
        _context.TillManagers.Add(request);
        var till = await _context.TillManagers.FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsFinish);
        if (till != null)
        {
            till.IsFinish = true;
            till.ToAt = DateTime.Now;
            _context.TillManagers.Update(till);
        }
        await _context.SaveChangesAsync();
        return request;
    }
    public async Task<TillManager> GetById(int id)
    {
        return await _context.TillManagers.FindAsync(id);
    }
    public async Task<TillManager> GetCurrentTillManager(int userId)
    {
        var till = await _context.TillManagers.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsFinish);
        if (till is null)
            return null;
        till.ToAmountAuto = await CaculateAmountInTill(userId);
        return till;
    }
    public async Task<string> Update(TillManager request)
    {
        var till = await _context.TillManagers.FindAsync(request.Id);
        if (till is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        till.FromAmount = request.FromAmount;
        till.IsDifferentMoney = request.IsDifferentMoney;
        till.IsFinish = request.IsFinish;
        if (request.IsFinish)
            till.ToAt = DateTime.Now;

        _context.TillManagers.Update(till);

        await _context.SaveChangesAsync();
        return string.Empty;
    }
    public async Task<double> CaculateAmountInTill(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
            throw new Exception("Không tìm thấy user");

        var till = await _context.TillManagers.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsFinish);
        if (till == null)
            throw new Exception("Không tìm thấy két");

        var billAmount = await _context.Bills.Where(x => x.UserCode == user.Username && x.TypePay == "TM" && x.CreatedDate >= till.FromAt && x.CreatedDate <= till.ToAt).SumAsync(x => x.TotalAmount + (x.Surcharge ?? 0));
        return billAmount + till.FromAmount;
    }
}
