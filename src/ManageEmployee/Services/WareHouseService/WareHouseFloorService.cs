using AutoMapper;
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
public class WareHouseFloorService : IWareHouseFloorService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WareHouseFloorService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PagingResult<WarehouseFloorPaging>> GetAll(PagingRequestModel param)
    {
        if (param.Page < 1)
            param.Page = 1;

        var data = _context.WareHouseFloors.Where(
            whf => param.SearchText.IsNullOrEmpty()
                || whf.Code.Contains(param.SearchText.Trim())
                || whf.Name.Contains(param.SearchText.Trim())
         ); ;

        var floors = await data.OrderBy(x => x.Id).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize)
            .Select(x => _mapper.Map<WarehouseFloorPaging>(x)).ToListAsync();

        foreach (var floor in floors)
        {
            var shelveInWarehouses = await _context.WareHouseFloorWithPositions
                .Join(
                        _context.WareHousePositions,
                        wareHouseShelvesWithFloors => wareHouseShelvesWithFloors.WareHousePositionId,
                        wareHousePositions => wareHousePositions.Id,
                        (wareHouseShelvesWithFloors, wareHousePositions) => new
                        {
                            Name = wareHousePositions.Name,
                            FloorId = wareHouseShelvesWithFloors.WareHouseFloorId
                        })
                .Where(x => x.FloorId == floor.Id)
                .Select(x => x.Name)
                .ToListAsync();

            floor.Positions = String.Join(", ", shelveInWarehouses);
        }

        return new PagingResult<WarehouseFloorPaging>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await data.CountAsync(),
            Data = floors
        };
    }
    public async Task<IEnumerable<WareHouseFloorGetAllModel>> GetAll()
    {
        return await _context.WareHouseFloors
           .Join(_context.WareHouseShelvesWithFloors,
                   b => b.Id,
                   d => d.WareHouseFloorId,
                   (b, d) => new {
                       wareHouseShelvesId = d.WareHouseShelvesId,
                       floor = b
                   })
           .Select(x => new WareHouseFloorGetAllModel
           {
               Id = x.floor.Id,
               Code = x.floor.Code,
               Name = x.floor.Name,
               WareHouseShelveId = x.wareHouseShelvesId
           })
           .ToListAsync();

    }

    public async Task<WarehouseFloorSetterModel> GetById(int id)
    {
        var item = await _context.WareHouseFloors.FindAsync(id);
        var itemOut = _mapper.Map<WarehouseFloorSetterModel>(item);
        itemOut.PositionIds = await _context.WareHouseFloorWithPositions.Where(x => x.WareHouseFloorId == id).Select(X => X.WareHousePositionId).ToListAsync();
        return itemOut;
    }

    public async Task Create(WarehouseFloorSetterModel param)
    {
        var floor = new WareHouseFloor();
        floor.Name = param.Name;
        floor.Code = param.Code;
        floor.Note = param.Note;
        _context.WareHouseFloors.Add(floor);
        _context.SaveChanges();
        if (param.PositionIds != null)
        {
            var shelveAdds = param.PositionIds.Select(x => new WareHouseFloorWithPosition()
            {
                WareHouseFloorId = floor.Id,
                WareHousePositionId = x,
            });

            await _context.WareHouseFloorWithPositions.AddRangeAsync(shelveAdds);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Update(WarehouseFloorSetterModel param)
    {
        var floor = await _context.WareHouseFloors.FindAsync(param.Id);

        if (floor == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        floor.Name = param.Name;
        floor.Code = param.Code;
        floor.Note = param.Note;
        _context.WareHouseFloors.Update(floor);

        var shelvelsDel = await _context.WareHouseFloorWithPositions.Where(x => x.WareHouseFloorId == param.Id).ToListAsync();
        _context.WareHouseFloorWithPositions.RemoveRange(shelvelsDel);

        if (param.PositionIds != null)
        {
            var shelveAdds = param.PositionIds.Select(x => new WareHouseFloorWithPosition()
            {
                WareHouseFloorId = floor.Id,
                WareHousePositionId = x,
            });

            await _context.WareHouseFloorWithPositions.AddRangeAsync(shelveAdds);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var itemFind = await _context.WareHouseFloors.FindAsync(id);
        if (itemFind != null)
        {
            _context.WareHouseFloors.Remove(itemFind);
            await _context.SaveChangesAsync();
        }
    }
}
