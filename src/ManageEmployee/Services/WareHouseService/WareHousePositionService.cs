using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.WareHouseEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.WareHouses;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.WareHouseService;
public class WareHousePositionService: IWareHousePositionService
{
    private readonly ApplicationDbContext _context;

    public WareHousePositionService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PagingResult<WareHousePosition>> GetAll(PagingRequestModel param)
    {
        if (param.Page < 1)
            param.Page = 1;

        var data = _context.WareHousePositions.Where(
            whp => param.SearchText.IsNullOrEmpty() 
                || whp.Code.Contains(param.SearchText.Trim()) 
                || whp.Name.Contains(param.SearchText.Trim())
         );

        return new PagingResult<WareHousePosition>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await data.CountAsync(),
            Data = await data.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync()
        };
    }
    public async Task<IEnumerable<WareHousePositionGetAllModel>> GetAll()
    {
        return await _context.WareHousePositions
           .Join(_context.WareHouseFloorWithPositions,
                   b => b.Id,
                   d => d.WareHousePositionId,
                   (b, d) => new {
                       WareHouseFloorId = d.WareHouseFloorId,
                       Position = b
                   })
           .Select(x => new WareHousePositionGetAllModel
           {
               Id = x.Position.Id,
               Code = x.Position.Code,
               Name = x.Position.Name,
               WareHouseFloorId = x.WareHouseFloorId
           })
           .ToListAsync();
    }

    public async Task<WareHousePosition> GetById(int id)
    {
        return await _context.WareHousePositions.FindAsync(id);
    }

    public async Task<WareHousePosition> Create(WareHousePosition param)
    {
        _context.WareHousePositions.Add(param);
        await _context.SaveChangesAsync();

        return param;
    }

    public async Task Update(WareHousePosition param)
    {
        var itemFind = await _context.WareHousePositions.FindAsync(param.Id);

        if (itemFind == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        _context.WareHousePositions.Update(param);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var itemFind = await _context.WareHousePositions.FindAsync(id);
        if (itemFind != null)
        {
            _context.WareHousePositions.Remove(itemFind);
            await _context.SaveChangesAsync();
        }
    }
}
