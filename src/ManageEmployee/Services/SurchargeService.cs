using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Surcharges;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class SurchargeService: ISurchargeService
{
    private readonly ApplicationDbContext _context;

    public SurchargeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<Surcharge>> GetAll(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 0;

            var datas =  _context.Surcharges
                .Where(x =>
                string.IsNullOrEmpty(keyword)
                || x.Name.ToLower().Contains(keyword.ToLower())
                || x.Code.ToLower().Contains(keyword.ToLower()))
                .Select(x => new Surcharge
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    Value = x.Value,
                    Type = x.Type,
                    Note = x.Note,
                });
            var result = new PagingResult<Surcharge>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Data = await datas.OrderByDescending(x => x.FromDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                TotalItems = await datas.CountAsync()
            };
            return result;
        }
        catch
        {
            return new PagingResult<Surcharge>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<Surcharge>()
            };
        }
    }

    public async Task<string> Create(Surcharge request)
    {
        try
        {
            var existCode = await _context.Surcharges.Where(
                x => x.Code.ToLower() == request.Code.ToLower()).FirstOrDefaultAsync();
            if (existCode != null)
            {
                return ErrorMessages.MenuKpiCodeAlreadyExist;
            }
            _context.Surcharges.Add(request);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Surcharge> GetById(int id)
    {
        var item = await _context.Surcharges.FindAsync(id);
        return item;
    }
    public async Task<Surcharge> GetCurrent()
    {
        return await _context.Surcharges.FirstOrDefaultAsync(x => x.FromDate <= DateTime.Now && x.ToDate > DateTime.Now);
    }
    public async Task<string> Update(Surcharge request)
    {
        var checkMenuKpiCode = await _context.Surcharges.Where(x => x.Code.ToLower() == request.Code.ToLower() && x.Id != request.Id).FirstOrDefaultAsync();
        if (checkMenuKpiCode != null && checkMenuKpiCode.Id != request.Id)
        {
            return ErrorMessages.MenuKpiCodeAlreadyExist;
        }
        _context.Surcharges.Update(request);

        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<string> Delete(int id)
    {
        var MenuKpi = await _context.Surcharges.FindAsync(id);
        if (MenuKpi != null)
        {
            _context.Surcharges.Remove(MenuKpi);
            await _context.SaveChangesAsync();
        }
        return string.Empty;
    }
}
