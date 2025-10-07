using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.DeskFloors;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Services;
public class DeskFloorService : IDeskFloorService
{
    private readonly ApplicationDbContext _context;

    public DeskFloorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DeskFloor>> GetAll()
    {
        var data = await _context.DeskFloors.Where(x => !x.IsDeleted).ToListAsync();
        return data;
    }

    public async Task<DeskFLoorPagingResult> GetPaging(DeskFLoorPagingationRequestModel param)
    {
        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page <= 0)
            param.Page = 1;

        var listDeskFloors = (from p in _context.DeskFloors
                              where (string.IsNullOrEmpty(param.SearchText) ? true : p.Name.ToLower().Contains(param.SearchText.ToLower().Trim()))
                              && p.IsDeleted == false
                              && (param.FloorId > 0 ? p.FloorId == param.FloorId : true)
                              select new DeskFloorModel
                              {
                                  Id = p.Id,
                                  Name = p.Name,
                                  Description = p.Description,
                                  FloorId = p.FloorId,
                                  IsDesk = p.IsDesk,
                                  IsChoose = p.IsChoose,
                                  NumberSeat = p.NumberSeat,
                                  Position = p.Position,
                                  Code = p.Code,
                                  Order = p.Order,
                                  StatusId = p.StatusId,
                              });
        if (!string.IsNullOrEmpty(param.SortField))
        {
            switch (param.SortField)
            {
                case "id":
                    listDeskFloors = param.isSort ? listDeskFloors.OrderByDescending(x => x.Id) : listDeskFloors.OrderBy(x => x.Id);
                    break;
                case "name":
                    listDeskFloors = param.isSort ? listDeskFloors.OrderByDescending(x => x.Name) : listDeskFloors.OrderBy(x => x.Name);
                    break;
                case "code":
                    listDeskFloors = param.isSort ? listDeskFloors.OrderByDescending(x => x.Code) : listDeskFloors.OrderBy(x => x.Code);
                    break;
            }
        }
        else
        {
            listDeskFloors = listDeskFloors.OrderBy(x => x.Order);
        }

        // check isFloor
        listDeskFloors = (param.IsFloor == true) ? listDeskFloors.Where(x => x.FloorId == 0) : listDeskFloors.Where(x => x.FloorId > 0);
        var deskLive = await _context.DeskFloors.FirstOrDefaultAsync(x => x.Code.ToLower() == "Live");
        var data = await listDeskFloors.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
        data.Add(new DeskFloorModel
        {
            Id = deskLive?.Id ?? 0,
            Name = deskLive?.Name ?? "Live",
            Code = deskLive?.Code ?? "LIVE",
            IsDesk = deskLive.IsDesk,
            IsChoose = deskLive.IsChoose,
            FloorId = deskLive?.FloorId ?? 0,
            StatusId = deskLive?.StatusId ?? 0,
            Order = deskLive?.Order ?? 0,
        });
        foreach (var item in data)
        {
            item.StatusName = await _context.Status.Where(x => x.Id == item.StatusId).Select(x => x.Name).FirstOrDefaultAsync();
            if (item.FloorId > 0)
            {
                item.FloorName = await _context.DeskFloors.Where(x => x.Id == item.FloorId).Select(x => x.Name).FirstOrDefaultAsync();
            }
        }
        
        return new DeskFLoorPagingResult()
        {
            pageIndex = param.Page,
            PageSize = param.PageSize,
            TotalItems = await listDeskFloors.CountAsync(),
            DeskFloors = data
        };
    }

    public async Task<DeskFloor> GetById(int id)
    {
            return await _context.DeskFloors.FindAsync(id);
    }

    public async Task<DeskFloor> GetByCode(string code)
    {
        return await _context.DeskFloors.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task Create(DeskFloor requests)
    {
            await _context.Database.BeginTransactionAsync();
            var exist = await _context.DeskFloors.SingleOrDefaultAsync(
                x => (x.Name == requests.Name || x.Code == requests.Code) && x.IsDesk == requests.IsDesk 
                    && x.FloorId == requests.FloorId && x.IsDeleted == false);
            if (exist != null)
            {
                throw new ErrorException(ErrorMessages.DeskFloorNameAlreadyExist);
            }

            _context.DeskFloors.Add(requests);
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
    }

    public async Task Update(DeskFloor requests)
    {
            using var trans = await _context.Database.BeginTransactionAsync();
            if (checkCodeDisable(requests))
                throw new ErrorException(ErrorMessages.FailedToDelete);

            var deskFloor = await _context.DeskFloors.SingleOrDefaultAsync(
                    x => x.Id == requests.Id && x.IsDeleted == false);
            if (deskFloor == null)
            {
                throw new ErrorException(ErrorMessages.DataNotFound);
            }
            var exist = await _context.DeskFloors.SingleOrDefaultAsync(
                x => (x.Name == requests.Name || x.Code == requests.Code) && x.IsDesk == requests.IsDesk 
                    && x.FloorId == requests.FloorId && x.IsDeleted == false && x.Id != requests.Id);
            if (exist != null)
            {
                throw new ErrorException(ErrorMessages.DeskFloorNameAlreadyExist);
            }
            deskFloor.FloorId = requests.FloorId;
            deskFloor.Name = requests.Name;
            deskFloor.IsDesk = requests.IsDesk;
            deskFloor.Description = requests.Description;
            deskFloor.NumberSeat = requests.NumberSeat;
            deskFloor.Position = requests.Position;
            deskFloor.Code = requests.Code;
            deskFloor.Order = requests.Order;
            deskFloor.StatusId = requests.StatusId;

            _context.DeskFloors.Update(deskFloor);
            await _context.SaveChangesAsync();
            await trans.CommitAsync();
    }

    public async Task Delete(int id)
    {
        var deskFloor = await _context.DeskFloors.FindAsync(id);
        if(checkCodeDisable(deskFloor))
            throw new ErrorException(ErrorMessages.FailedToDelete);

        if (deskFloor != null)
        {
            deskFloor.IsDeleted = true;
            _context.DeskFloors.Update(deskFloor);
            await _context.SaveChangesAsync();
        }
    }
    public void UpdateDeskChoose(int id, bool isChoose)
    {
        var deskFloor = _context.DeskFloors.FirstOrDefault(x => x.Id == id && x.Code.ToLower() != "live");
        if (deskFloor != null)
        {
            deskFloor.IsChoose = isChoose;
            _context.DeskFloors.Update(deskFloor);
            _context.SaveChanges();
        }
    }
    public bool checkCodeDisable(DeskFloor desk)
    {
        string[] listCode = { "LIVE", "Floor"};
        if(listCode.Contains(desk.Code))
            return true;
        return false;
    }

    public async Task<IEnumerable<DeskFloor>> GetListDeskFree()
    {
        return await _context.DeskFloors.Where(x => !x.IsDeleted && !x.IsChoose).ToListAsync();
    }

    public async Task ResetDeskChoose()
    {
        var deskFloors = await _context.DeskFloors.Where(x => x.IsChoose).ToListAsync();
        deskFloors = deskFloors.ConvertAll(x =>
        {
            x.IsChoose = false;
            return x;
        });
        _context.DeskFloors.UpdateRange(deskFloors);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatus(int id, int statusId)
    {
        var desk = await _context.DeskFloors.FindAsync(id);
        if (desk is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        //validate Status
        var status = await _context.Status.FirstOrDefaultAsync(x => x.Id == statusId && x.Type == StatusTypeEnum.Room);
        if (status is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        desk.StatusId = statusId;
        _context.DeskFloors.Update(desk);
        await _context.SaveChangesAsync();
    }
}
