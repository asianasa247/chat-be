using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.InOutServices;

public class WorkingDayService : IWorkingDayService
{
    private readonly ApplicationDbContext _context;

    public WorkingDayService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<WorkingDay>> GetPaging(PagingRequestModel param)
    {
        var query = _context.WorkingDays;
        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var totalItem = await query.CountAsync();
        return new PagingResult<WorkingDay>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<WorkingDayModel> GetDetail(int id)
    {
        var item = await _context.WorkingDays.FirstOrDefaultAsync(x => x.Id == id);
        return new WorkingDayModel
        {
            Id = id,
            Year = item.Year,
            Days = item.Days.Split(",").ToList(),
            Holidays = item.Holidays.Split(",").ToList()
        };
    }

    public async Task Create(WorkingDayModel form)
    {
        await Validate(form);
        var item = MapData(form);
        await _context.WorkingDays.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(WorkingDayModel form)
    {
        await Validate(form);

        var itemCheck = await _context.WorkingDays.AnyAsync(x => x.Id == form.Id);
        if (!itemCheck)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var item = MapData(form);
        _context.WorkingDays.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var item = await _context.WorkingDays.FirstOrDefaultAsync(x => x.Id == id);
        if (item is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        _context.WorkingDays.Remove(item);
        await _context.SaveChangesAsync();
    }

    private async Task Validate(WorkingDayModel form)
    {
        var isUniqueYear = await _context.WorkingDays.AnyAsync(x => x.Year == form.Year && x.Id != form.Id);
        if (isUniqueYear)
        {
            throw new ErrorException(ErrorMessages.YearShouldUnique);
        }
    }

    private WorkingDay MapData(WorkingDayModel form)
    {
        return new WorkingDay
        {
            Id = form.Id,
            Year = form.Year,
            Days = string.Join(",", form.Days),
            Holidays = string.Join(",", form.Holidays)
        };
    }
}