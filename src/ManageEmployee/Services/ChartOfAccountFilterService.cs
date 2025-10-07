using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class ChartOfAccountFilterService: IChartOfAccountFilterService
{
    private readonly ApplicationDbContext _context;
    public ChartOfAccountFilterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(ChartOfAccountFilter param)
    {
        _context.ChartOfAccountFilters.Add(param);
        await _context.SaveChangesAsync();
    }
    public async Task Update(ChartOfAccountFilter param)
    {
        _context.ChartOfAccountFilters.Update(param);
        await _context.SaveChangesAsync();
    }
    public async Task<List<ChartOfAccountFilter>> GetList()
    {
       return await _context.ChartOfAccountFilters.ToListAsync();
    }
    public async Task<PagingResult<ChartOfAccountFilter>> GetPaging(PagingRequestModel form)
    {
        if (form.Page < 1)
            form.Page = 1;
        var query =  _context.ChartOfAccountFilters.Where(x => string.IsNullOrEmpty(form.SearchText) || x.Name == form.SearchText);

        return new PagingResult<ChartOfAccountFilter>
        {
            PageSize = form.PageSize,
            CurrentPage = form.Page,
            TotalItems = await query.CountAsync(),
            Data = await query.Skip(form.PageSize * (form.Page - 1)).Take(form.PageSize).ToListAsync()
        };
    }
    public async Task Delete(int id)
    {
        var item = await _context.ChartOfAccountFilters.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        _context.ChartOfAccountFilters.Remove(item);
        await _context.SaveChangesAsync();
    }
    public async Task<ChartOfAccountFilter> GetDetail(int id)
    {
        var item = await _context.ChartOfAccountFilters.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        return item;
    }
}
