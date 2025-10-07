using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.HotelEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.ViewModels.HotelModels;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.HotelServices;

public class RoomConfigureTypeService : IRoomConfigureTypeService
{
    private readonly ApplicationDbContext _context;

    public RoomConfigureTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomConfigureTypeModel>> GetList()
    {
        return await _context.RoomConfigureTypes.Select(x => new RoomConfigureTypeModel
        {
            NameVn = x.NameVn,
            NameKo = x.NameKo,
            NameEn = x.NameEn,
            Id = x.Id,
            Code = x.Code
        }).ToListAsync();
    }

    public async Task<RoomConfigureTypeModel> GetById(int id)
    {
        var item = await _context.RoomConfigureTypes.Where(x => x.Id == id).Select(x => new RoomConfigureTypeModel
        {
            NameVn = x.NameVn,
            NameKo = x.NameKo,
            NameEn = x.NameEn,
            Code = x.Code,
            Type = x.Type,
            Id = x.Id
        }).FirstOrDefaultAsync();
        item.Items = await _context.RoomConfigures.Where(x => x.RoomConfigureTypeId == id)
            .Select(x => new RoomConfigureModel
            {
                NameVn = x.NameVn,
                NameKo = x.NameKo,
                NameEn = x.NameEn,
                Code = x.Code,
                Id = x.Id,
                RoomConfigureTypeId = x.RoomConfigureTypeId
            }).ToListAsync();

        return item;
    }

    public async Task<PagingResult<RoomConfigureTypeModel>> GetPaging(PagingRequestModel param)
    {
        if (param.Page < 1)
            param.Page = 1;

        var query = _context.RoomConfigureTypes.Where(x => string.IsNullOrEmpty(param.SearchText) || x.NameVn.Contains(param.SearchText) || x.Code.Contains(param.SearchText));
        return new PagingResult<RoomConfigureTypeModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip(param.PageSize * (param.Page - 1)).Take(param.PageSize).Select(x => new RoomConfigureTypeModel
            {
                NameVn = x.NameVn,
                NameKo = x.NameKo,
                NameEn = x.NameEn,
                Id = x.Id,
                Code = x.Code
            }).ToListAsync(),
            TotalItems = await query.CountAsync(),
        };
    }

    public async Task Create(RoomConfigureTypeModel form)
    {
        var roomConfigureType = new RoomConfigureType
        {
            NameVn = form.NameVn,
            NameKo = form.NameKo,
            NameEn = form.NameEn,
            Code = form.Code,
            Type = form.Type
        };
        await _context.AddAsync(roomConfigureType);
        await _context.SaveChangesAsync();

        var roomConfigures = form.Items.Select(x => new RoomConfigure
        {
            NameVn = x.NameVn,
            NameKo = x.NameKo,
            NameEn = x.NameEn,
            Code = x.Code,
            RoomConfigureTypeId = roomConfigureType.Id
        });
        await _context.AddRangeAsync(roomConfigures);
        await _context.SaveChangesAsync();
    }

    public async Task Update(RoomConfigureTypeModel form)
    {
        var roomConfigureType = await _context.RoomConfigureTypes.FindAsync(form.Id);
        if (roomConfigureType is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        roomConfigureType.NameVn = form.NameVn;
        roomConfigureType.NameKo = form.NameKo;
        roomConfigureType.NameEn = form.NameEn;
        roomConfigureType.Code = form.Code;
        roomConfigureType.Type = form.Type;
        _context.Update(roomConfigureType);

        var roomConfigureDels = await _context.RoomConfigures.Where(x => x.RoomConfigureTypeId == roomConfigureType.Id).ToListAsync();
        _context.RoomConfigures.RemoveRange(roomConfigureDels);

        var roomConfigures = form.Items.Select(x => new RoomConfigure
        {
            NameVn = x.NameVn,
            NameKo = x.NameKo,
            NameEn = x.NameEn,
            Code = x.Code,
            RoomConfigureTypeId = roomConfigureType.Id
        });
        await _context.AddRangeAsync(roomConfigures);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var amenity = await _context.RoomConfigureTypes.FindAsync(id);
        if (amenity is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        _context.Remove(amenity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RoomConfigureTypeModel>> GetRoomConfigureForType(RoomConfigureTypeEnum type)
    {
        var roomConfigureTypes = await _context.RoomConfigureTypes.Where(x => x.Type == type).ToListAsync();
        var roomConfigureTypeIds = roomConfigureTypes.Select(x => x.Id);
        var roomConfigures = await _context.RoomConfigures.Where(x => roomConfigureTypeIds.Contains(x.RoomConfigureTypeId)).Select(x => new RoomConfigureModel
        {
            Id = x.Id,
            RoomConfigureTypeId = x.RoomConfigureTypeId,
            Code = x.Code,
            NameVn = x.NameVn,
        }).ToListAsync();

        return roomConfigureTypes.Select(x => new RoomConfigureTypeModel
        {
            NameVn = x.NameVn,
            NameKo = x.NameKo,
            NameEn = x.NameEn,
            Id = x.Id,
            Code = x.Code,
            Items = roomConfigures.Where(a => a.RoomConfigureTypeId == x.Id).ToList()
        });
    }
}