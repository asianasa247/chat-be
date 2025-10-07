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
public class WareHouseShelvesService: IWareHouseShelvesService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WareHouseShelvesService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PagingResult<WareHouseShelvesPaging>> GetAll(PagingRequestModel param)
    {
        if (param.Page < 1)
            param.Page = 1;

        var data = _context.WareHouseShelves.Where(
            whs => param.SearchText.IsNullOrEmpty()
                || whs.Code.Contains(param.SearchText.Trim())
                || whs.Name.Contains(param.SearchText.Trim())
         ); ;

        var shelves = await data.OrderBy(x => x.Id).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize)
            .Select(x => _mapper.Map<WareHouseShelvesPaging>(x)).ToListAsync();

        foreach (var shelve in shelves)
        {
            var shelveInWarehouses = await _context.WareHouseShelvesWithFloors
                .Join(
                        _context.WareHouseFloors,
                        wareHouseShelvesWithFloors => wareHouseShelvesWithFloors.WareHouseFloorId,
                        wareHouseFloors => wareHouseFloors.Id,
                        (wareHouseShelvesWithFloors, wareHouseFloors) => new
                        {
                            Name = wareHouseFloors.Name,
                            ShelveId = wareHouseShelvesWithFloors.WareHouseShelvesId
                        })
                .Where(x => x.ShelveId == shelve.Id)
                .Select(x => x.Name)
                .ToListAsync();

            shelve.Floors = String.Join(", ", shelveInWarehouses);
        }

        return new PagingResult<WareHouseShelvesPaging>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await data.CountAsync(),
            Data = shelves
        };
    }
    public async Task<IEnumerable<WareHouseShelvesGetAllModel>> GetAll()
    {
        return await _context.WareHouseShelves
            .Join(_context.WareHouseWithShelves,
                    b => b.Id,
                    d => d.WareHouseShelveId,
                    (b, d) => new { wareHouseId = d.WareHouseId,
                    shelves = b})
            .Select(x => new WareHouseShelvesGetAllModel
            {
                Id = x.shelves.Id,
                Code = x.shelves.Code,
                Name = x.shelves.Name,
                WareHouseId = x.wareHouseId
            })
            .ToListAsync();
    }

    public async Task<WarehouseShelveSetterModel> GetById(int id)
    {
        var item = await _context.WareHouseShelves.FindAsync(id);
        var itemOut = _mapper.Map<WarehouseShelveSetterModel>(item);
        itemOut.FloorIds = await _context.WareHouseShelvesWithFloors.Where(x => x.WareHouseShelvesId == id).Select(X => X.WareHouseFloorId).ToListAsync();
        return itemOut;
    }

    public async Task Create(WarehouseShelveSetterModel param)
    {
        var floor = new WareHouseShelves();
        floor.Name = param.Name;
        floor.Code = param.Code;
        floor.Note = param.Note;
        floor.OrderVertical = param.OrderVertical;
        floor.OrderHorizontal = param.OrderHorizontal;
        _context.WareHouseShelves.Add(floor);
        _context.SaveChanges();

        if (param.FloorIds != null)
        {
            var shelveAdds = param.FloorIds.Select(x => new WareHouseShelvesWithFloor()
            {
                WareHouseShelvesId = floor.Id,
                WareHouseFloorId = x,
            });

            await _context.WareHouseShelvesWithFloors.AddRangeAsync(shelveAdds);
        }
        await _context.SaveChangesAsync();

    }

    public async Task Update(WarehouseShelveSetterModel param)
    {
        var floor = await _context.WareHouseShelves.FindAsync(param.Id);

        if (floor == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        floor.Name = param.Name;
        floor.Code = param.Code;
        floor.Note = param.Note;
        floor.OrderVertical = param.OrderVertical;
        floor.OrderHorizontal = param.OrderHorizontal;
        _context.WareHouseShelves.Update(floor);
        _context.SaveChanges();

        var shelvelsDel = await _context.WareHouseShelvesWithFloors.Where(x => x.WareHouseShelvesId == param.Id).ToListAsync();
        _context.WareHouseShelvesWithFloors.RemoveRange(shelvelsDel);

        if (param.FloorIds != null)
        {
            var shelveAdds = param.FloorIds.Select(x => new WareHouseShelvesWithFloor()
            {
                WareHouseShelvesId = floor.Id,
                WareHouseFloorId = x,
            });

            await _context.WareHouseShelvesWithFloors.AddRangeAsync(shelveAdds);

        }
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var itemFind = await _context.WareHouseShelves.FindAsync(id);
        if (itemFind != null)
        {
            _context.WareHouseShelves.Remove(itemFind);
           await  _context.SaveChangesAsync();
        }
    }
}
