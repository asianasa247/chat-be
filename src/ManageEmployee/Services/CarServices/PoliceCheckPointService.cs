using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.CarServices;
public class PoliceCheckPointService: IPoliceCheckPointService
{
    private readonly ApplicationDbContext _context;

    public PoliceCheckPointService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Create(PoliceCheckPoint form)
    {
        await _context.PoliceCheckPoints.AddAsync(form);
        await _context.SaveChangesAsync();
    }

    public async Task Update(PoliceCheckPoint form)
    {
        _context.PoliceCheckPoints.Update(form);
        await _context.SaveChangesAsync();
    }

    public async Task<PoliceCheckPoint> GetDetail(int id)
    {
        return await _context.PoliceCheckPoints.FindAsync(id);
    }

    public async Task<PagingResult<PoliceCheckPoint>> GetPaging(PagingRequestModel searchRequest)
    {
        var query = _context.PoliceCheckPoints.Where(x => string.IsNullOrEmpty(searchRequest.SearchText) || x.Name.Contains(searchRequest.SearchText));
        return new PagingResult<PoliceCheckPoint>
        {
            Data = await query.Skip(searchRequest.Page * searchRequest.PageSize).Take(searchRequest.PageSize).ToListAsync(),
            CurrentPage = searchRequest.Page,
            PageSize = searchRequest.PageSize,
            TotalItems = await query.CountAsync()
        };
    }
    public async Task Delete(int id)
    {

        var item = await _context.PoliceCheckPoints.FindAsync(id);
        if (item is null)
        {
            throw new ErrorException(ErrorMessages.DataExist);
        }
        _context.PoliceCheckPoints.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PoliceCheckPoint>> GetAll()
    {
        return await _context.PoliceCheckPoints.ToListAsync();
    }

}
