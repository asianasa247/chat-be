using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.Entities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.WareHouseEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.WareHouses;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class WarehouseService : IWarehouseService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WarehouseService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<WarehousePaging>> GetAll(DepartmentRequest param)
    {
        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page < 0)
            param.Page = 1;

        var result = new PagingResult<WarehousePaging>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
        };

        var query = _context.Warehouses.Where(x => !x.IsDelete)
            .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText)
            || x.Code.Contains(param.SearchText));


        var warehouses = await query.OrderBy(x => x.Id).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize)
            .Select(x => _mapper.Map<WarehousePaging>(x)).ToListAsync();

        foreach(var warehouse in warehouses)
        {
            var shelveInWarehouses = await _context.WareHouseWithShelves
                .Join(
                        _context.WareHouseShelves,
                        wareHouseWithShelves => wareHouseWithShelves.WareHouseShelveId,
                        wareHouseShelves => wareHouseShelves.Id,
                        (wareHouseWithShelves, wareHouseShelves) => new
                        {
                            Name = wareHouseShelves.Name,
                            WareHouseId = wareHouseWithShelves.WareHouseId
                        })
                .Where(x => x.WareHouseId == warehouse.Id)
                .Select(x => x.Name)
                .ToListAsync();

            warehouse.Shevles = String.Join(", ", shelveInWarehouses);
        }

        result.TotalItems = await query.CountAsync();
        result.Data = warehouses;
        return result;
    }

    public IEnumerable<Warehouse> GetAll()
    {
        return _context.Warehouses
            .Where(x => !x.IsDelete)
                .OrderBy(x => x.Name);
    }

    public async Task<WarehouseSetterModel> GetById(int id)
    {
        var item = await _context.Warehouses.FindAsync(id);
        var itemOut = _mapper.Map<WarehouseSetterModel>(item);
        itemOut.ShelveIds = await _context.WareHouseWithShelves.Where(x => x.WareHouseId == id).Select(X => X.WareHouseShelveId).ToListAsync();
        return itemOut;
    }

    public async Task Create(WarehouseSetterModel param, int userId, int yearFilter)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        var isExisted = await _context.Warehouses.AnyAsync(x => x.Code.Trim().ToLower() == param.Code.Trim().ToLower() && !x.IsDelete);
        if (isExisted)
        {
            throw new ErrorException(ResultErrorConstants.WAREHOUSE_EXIST);
        }

        var warehouse = new Warehouse();
        warehouse.Name = param.Name;
        warehouse.Code = param.Code;
        warehouse.ManagerName = param.ManagerName;
        warehouse.UserCreated = userId;
        warehouse.UserUpdated = userId;
        warehouse.BranchId = param.BranchId;
        warehouse.IsSyncChartOfAccount = param.IsSyncChartOfAccount;

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        if (param.ShelveIds != null)
        {
            var shelveAdds = param.ShelveIds.Select(x => new WareHouseWithShelves()
            {
                WareHouseId = warehouse.Id,
                WareHouseShelveId = x,
            });

            await _context.WareHouseWithShelves.AddRangeAsync(shelveAdds);
        }
        await SyncChartOfAccount(warehouse, yearFilter);
        await _context.SaveChangesAsync();
    }

    public async Task Update(WarehouseSetterModel param, int userId, int yearFilter)
    {
        var warehouse = await _context.Warehouses.FindAsync(param.Id);

        if (warehouse == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        bool isSyncAccount = warehouse.IsSyncChartOfAccount;
        var checkCode = _context.Warehouses.FirstOrDefault(x => x.Id != param.Id && x.Code.Trim().ToLower() == (param.Code.Trim().ToLower()));
        if (checkCode != null)
        {
            throw new ErrorException(ResultErrorConstants.WAREHOUSE_EXIST);
        }

        warehouse.Name = param.Name;
        warehouse.Code = param.Code;
        warehouse.ManagerName = param.ManagerName;
        warehouse.UpdatedAt = DateTime.Now;
        warehouse.UserUpdated = userId;
        warehouse.BranchId = param.BranchId;
        warehouse.IsSyncChartOfAccount = param.IsSyncChartOfAccount;

        _context.Warehouses.Update(warehouse);
        var shelvelsDel = await _context.WareHouseWithShelves.Where(x => x.WareHouseId == param.Id).ToListAsync();
        _context.WareHouseWithShelves.RemoveRange(shelvelsDel);
        if (param.ShelveIds != null)
        {
            var shelveAdds = param.ShelveIds.Select(x => new WareHouseWithShelves()
            {
                WareHouseId = warehouse.Id,
                WareHouseShelveId = x,
            });

            await _context.WareHouseWithShelves.AddRangeAsync(shelveAdds);
        }
        if(!isSyncAccount)
            await SyncChartOfAccount(warehouse, yearFilter);

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);

        var isCheckMemberHaveWarehoue = await _context.Users.Where(x => !x.IsDelete && x.WarehouseId == id).AnyAsync();
        var isAccount = await _context.ChartOfAccounts.Where(x => x.WarehouseCode == warehouse.Code).AnyAsync();
        if (isCheckMemberHaveWarehoue || isAccount)
        {
            if (warehouse != null)
            {
                warehouse.IsDelete = true;
                warehouse.DeleteAt = DateTime.Now;
                _context.Warehouses.Update(warehouse);
            }
        }
        else
        {
            _context.Warehouses.Remove(warehouse);

        }
        await _context.SaveChangesAsync();
    }

    private async Task SyncChartOfAccount(Warehouse warehouse, int yearFilter)
    {
        if (warehouse.IsSyncChartOfAccount)
        {
            var wareHouseSynced = await _context.Warehouses.FirstOrDefaultAsync(x => x.IsSyncChartOfAccount && !x.IsDelete && x.Id != warehouse.Id);
            if (wareHouseSynced is null)
                return;

            var accountForSyncs = await _context.ChartOfAccounts.Where(x => x.WarehouseCode == warehouse.Code).ToListAsync();
            var accountToSyncs = await _context.ChartOfAccounts.Where(x => x.WarehouseCode == wareHouseSynced.Code).ToListAsync();
            List<ChartOfAccount> accountAdds = new List<ChartOfAccount>();

            foreach(var accountToSync in accountToSyncs)
            {
                var account_new = accountForSyncs.FirstOrDefault(x => x.Code == accountToSync.Code && x.ParentRef == accountToSync.ParentRef);
                if(account_new is null)
                {
                    account_new = new ChartOfAccount();
                    account_new.Code = accountToSync.Code;
                    account_new.Name = accountToSync.Name;
                    account_new.ParentRef = accountToSync.ParentRef;
                    account_new.Duration = accountToSync.Duration;
                    account_new.AccGroup = accountToSync.AccGroup;
                    account_new.Classification = accountToSync.Classification;
                    account_new.Protected = accountToSync.Protected;
                    account_new.WarehouseCode = warehouse.Code;
                    account_new.WarehouseName = warehouse.Name;
                    account_new.Type = accountToSync.Type;
                    account_new.StockUnit = accountToSync.StockUnit;
                    account_new.Year = yearFilter;
                    accountAdds.Add(account_new);
                }
            }
            await _context.ChartOfAccounts.AddRangeAsync(accountAdds);
            await _context.SaveChangesAsync();
        }
    }
}