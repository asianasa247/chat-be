using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChartOfAccountServices;
public class CharOfAccountSyncService: ICharOfAccountSyncService
{
    private readonly ApplicationDbContext _context;
    private List<ChartOfAccount> _chartOfAccountAdds = new List<ChartOfAccount>();
    private List<ChartOfAccount> _chartOfAccountUpdates = new List<ChartOfAccount>();

    public CharOfAccountSyncService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<ChartOfAccount> accountAdds, List<ChartOfAccount> accountUpdates)> SyncAccountGroupAsync(ChartOfAccount entity, string grandParentRef, string parentCode, List<Warehouse> warehouses, ChartOfAccount accountParent, int year)
    {
        var accountGroupLink = await _context.GetChartOfAccountGroupLink(year)
                .FirstOrDefaultAsync(x => entity.Type == 6 ? x.CodeChartOfAccount == grandParentRef : x.CodeChartOfAccount == entity.ParentRef);
        if (accountGroupLink != null)
        {
            var accountGroupLinkCodes = await _context.GetChartOfAccountGroupLink(year).Where(x => x.CodeChartOfAccountGroup == accountGroupLink.CodeChartOfAccountGroup).Select(x => x.CodeChartOfAccount).ToListAsync();
            var accounts = await _context.GetChartOfAccount(year).Where(x => accountGroupLinkCodes.Contains(x.Code)).ToListAsync();
            await SyncAccountFromGroupLink(accounts, warehouses, entity, grandParentRef, parentCode, year);
        }

        await SyncAccountSameLevelAsync(entity, warehouses, grandParentRef, parentCode, year);
        return (_chartOfAccountAdds, _chartOfAccountUpdates);
    }

    public async Task<(List<ChartOfAccount> accountAdds, List<ChartOfAccount> accountUpdates)> SyncListAccountGroupAsync(List<ChartOfAccount> entities, string parentCode, List<Warehouse> warehouses, List<ChartOfAccount> listAccount, int year)
    {
        var accountGroupLink = await _context.GetChartOfAccountGroupLink(year).FirstOrDefaultAsync(x => x.CodeChartOfAccount == parentCode);
        if (accountGroupLink == null)
            return (_chartOfAccountAdds, _chartOfAccountUpdates);

        var accountGroupLinkCodes = await _context.GetChartOfAccountGroupLink(year).Where(x => x.CodeChartOfAccountGroup == accountGroupLink.CodeChartOfAccountGroup).Select(x => x.CodeChartOfAccount).ToListAsync();
        if (!accountGroupLinkCodes.Any())
            return (_chartOfAccountAdds, _chartOfAccountUpdates);

        var accounts = listAccount.Where(x => accountGroupLinkCodes.Contains(x.Code)).ToList();
        foreach (var entity in entities)
        {
            string grandParentRef = "";
            if (entity.ParentRef.Contains(":"))
            {
                string[] segments = entity.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
                parentCode = segments[1];
                grandParentRef = segments[0];
            }

            if (accountGroupLink != null)
            {
                await SyncAccountFromGroupLink(accounts, warehouses, entity, grandParentRef, parentCode, year);
            }

            await SyncAccountSameLevelAsync(entity, warehouses, grandParentRef, parentCode, year, entities);
        }
        return (_chartOfAccountAdds, _chartOfAccountUpdates);
    }

    private async Task SyncAccountFromGroupLink(List<ChartOfAccount> accounts, List<Warehouse> warehouses, ChartOfAccount entity, string grandParentRef, string parentCode, int year)
    {
        if (entity.Type == 5)
        {
            SyncAccountTypeIs5(accounts, warehouses, entity, year);
        }
        else
        {
            await SyncAccountTypeIs6Async(accounts, warehouses, entity, grandParentRef, parentCode, year);
        }
    }

    private void SyncAccountTypeIs5(List<ChartOfAccount> accounts, List<Warehouse> warehouses, ChartOfAccount entity, int year)
    {
        foreach (var account in accounts)
        {
            if (entity.ParentRef == account.Code)
                continue;

            account.HasDetails = true;
            account.HasChild = false;
            account.DisplayDelete = false;
            account.Year = year;

            var accountCheck = _chartOfAccountUpdates.Find(x => x.Id == account.Id);
            if (account is null)
            {
                _chartOfAccountUpdates.Add(account);
            } 
            else
            {
                _context.ChartOfAccounts.Update(account);
                _context.SaveChanges();
            }

            var account_new = new ChartOfAccount();
            account_new.Code = entity.Code;
            account_new.Name = entity.Name;
            account_new.ParentRef = account.Code;
            account_new.Duration = account.Duration;
            account_new.AccGroup = account.AccGroup;
            account_new.Classification = account.Classification;
            account_new.Protected = account.Protected;
            account_new.Type = entity.Type;
            account_new.StockUnit = entity.StockUnit;
            account_new.Year = year;

            if (account.AccGroup == 3)// && string.IsNullOrEmpty(accountParent.WarehouseCode))
            {
                account_new.WarehouseCode = entity.WarehouseCode;
                account_new.WarehouseName = entity.WarehouseName;
                foreach (var warehouse in warehouses)
                {
                    if (warehouse.Code == entity.WarehouseCode)
                        continue;

                    var account_new_warehouse = new ChartOfAccount();
                    account_new_warehouse.Code = entity.Code;
                    account_new_warehouse.Name = entity.Name;
                    account_new_warehouse.ParentRef = account.Code;
                    account_new_warehouse.Duration = account.Duration;
                    account_new_warehouse.AccGroup = account.AccGroup;
                    account_new_warehouse.Classification = account.Classification;
                    account_new_warehouse.Protected = account.Protected;
                    account_new_warehouse.WarehouseCode = warehouse.Code;
                    account_new_warehouse.WarehouseName = warehouse.Name;
                    account_new_warehouse.Type = entity.Type;
                    account_new_warehouse.StockUnit = entity.StockUnit;
                    account_new_warehouse.Year = year;

                    _chartOfAccountAdds.Add(account_new_warehouse);
                }
            }
            else
            {

                // check account
                var account_new_check = _chartOfAccountAdds.FirstOrDefault(x => x.Code == entity.Code && x.ParentRef == account.Code);
                if(account_new_check is not null)
                {
                    continue;
                }
            }
            _chartOfAccountAdds.Add(account_new);
        }

    }

    private async Task SyncAccountTypeIs6Async(List<ChartOfAccount> accounts, List<Warehouse> warehouses, ChartOfAccount entity, string grandParentRef, string parentCode, int year)
    {
        foreach (var account in accounts)
        {
            if (account.Code == grandParentRef)
                continue;
            var accountParentGroup = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.ParentRef == account.Code
            && x.Code == parentCode && (x.WarehouseCode == entity.WarehouseCode || string.IsNullOrEmpty(x.WarehouseCode)));
            if (accountParentGroup is null)
            {
                accountParentGroup = _chartOfAccountAdds.FirstOrDefault(x => x.ParentRef == account.Code
                                    && x.Code == parentCode && (x.WarehouseCode == entity.WarehouseCode || string.IsNullOrEmpty(x.WarehouseCode)));
                if (accountParentGroup is null)
                {

                    var accountgrandParent = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == account.Code);
                    if (accountgrandParent is null)
                    {
                        throw new ErrorException($"Chưa có tài khoản {account.Code}");
                    }
                    // add new account
                    accountParentGroup = new ChartOfAccount
                    {
                        HasChild = false,
                        HasDetails = true,
                        DisplayDelete = true,
                        Code = parentCode,
                        ParentRef = account.Code,
                        WarehouseCode = entity.WarehouseCode,
                        Duration = accountgrandParent.Duration,
                        AccGroup = accountgrandParent.AccGroup,
                        Classification = accountgrandParent.Classification,
                        Protected = accountgrandParent.Protected,
                        Type = 5,
                        Year = year,
                    };
                    _chartOfAccountAdds.Add(accountParentGroup);
                }
                else
                {
                    accountParentGroup.HasChild = false;
                    accountParentGroup.HasDetails = true;
                    accountParentGroup.DisplayDelete = false;
                }
            }
            else
            {
                accountParentGroup.HasChild = false;
                accountParentGroup.HasDetails = true;
                accountParentGroup.DisplayDelete = false;

                var accountCheck = _chartOfAccountUpdates.Find(x => x.Id == account.Id);
                if (account is null)
                {
                    _chartOfAccountUpdates.Add(account);
                } 
               
                _context.ChartOfAccounts.Update(accountParentGroup);
                _context.SaveChanges();
                
            }
           
            var account_new = new ChartOfAccount();
            account_new.Code = entity.Code;
            account_new.Name = entity.Name;
            account_new.ParentRef = account.Code + ":" + parentCode;
            account_new.Duration = accountParentGroup.Duration;
            account_new.AccGroup = accountParentGroup.AccGroup;
            account_new.Classification = accountParentGroup.Classification;
            account_new.Protected = accountParentGroup.Protected;
            account_new.Type = entity.Type;
            account_new.StockUnit = entity.StockUnit;
            account_new.Year = year;
            

            if (account.AccGroup == 3)// && string.IsNullOrEmpty(accountParent.WarehouseCode))
            {
                account_new.WarehouseCode = entity.WarehouseCode;
                account_new.WarehouseName = entity.WarehouseName;

                foreach (var warehouse in warehouses)
                {
                    if (warehouse.Code == entity.WarehouseCode)
                        continue;
                    // find parent
                    var accountParentWarehouse = await _context.GetChartOfAccount(year).AsNoTracking().FirstOrDefaultAsync(x => x.ParentRef == account.Code && x.Code == parentCode && x.WarehouseCode == warehouse.Code);
                    if (accountParentWarehouse is not null)
                    {
                        accountParentWarehouse.HasChild = false;
                        accountParentWarehouse.HasDetails = true;
                        accountParentWarehouse.DisplayDelete = false;
                        var accountCheck = _chartOfAccountUpdates.Find(x => x.Id == account.Id);
                        if (account is null)
                        {
                            _chartOfAccountUpdates.Add(account);
                        }
                    }

                    var account_new_warehouse = new ChartOfAccount();
                    account_new_warehouse.Code = entity.Code;
                    account_new_warehouse.Name = entity.Name;
                    account_new_warehouse.ParentRef = account.Code + ":" + parentCode;
                    account_new_warehouse.Duration = accountParentGroup.Duration;
                    account_new_warehouse.AccGroup = accountParentGroup.AccGroup;
                    account_new_warehouse.Classification = accountParentGroup.Classification;
                    account_new_warehouse.Protected = accountParentGroup.Protected;
                    account_new_warehouse.WarehouseCode = warehouse.Code;
                    account_new_warehouse.WarehouseName = warehouse.Name;
                    account_new_warehouse.Type = entity.Type;
                    account_new_warehouse.StockUnit = entity.StockUnit;
                    account_new_warehouse.Year = year;

                    _chartOfAccountAdds.Add(account_new_warehouse);
                }
            }
            else
            {

                // check account
                var account_new_check = _chartOfAccountAdds.FirstOrDefault(x => x.Code == entity.Code && x.ParentRef == (account.Code + ":" + parentCode));
                if (account_new_check is not null)
                {
                    continue;
                }
            }
            _chartOfAccountAdds.Add(account_new);
        }

    }

    private async Task SyncAccountSameLevelAsync(ChartOfAccount entity, List<Warehouse> warehouses, string grandParentRef, string parentCode, int year, List<ChartOfAccount> entities = null)
    {
        if(entities is null)
        {
            entities = new List<ChartOfAccount>();
        }

        if (entity.AccGroup == 3)// && string.IsNullOrEmpty(accountParent.WarehouseCode))
        {
            // Synchronize accounts at the same level with the current account with warehouses
            foreach (var warehouse in warehouses)
            {
                if (warehouse.Code == entity.WarehouseCode)
                    continue;

                // find parent
                var accountParentWarehouse = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.ParentRef == grandParentRef && x.Code == parentCode && x.WarehouseCode == warehouse.Code);
                
                if (accountParentWarehouse is not null)
                {
                    var accountCheck = _chartOfAccountUpdates.Find(x => x.Id == accountParentWarehouse.Id);
                    if (accountCheck is null)
                    {
                        accountParentWarehouse.HasChild = false;
                        accountParentWarehouse.HasDetails = true;
                        accountParentWarehouse.DisplayDelete = false;
                        _chartOfAccountUpdates.Add(accountParentWarehouse);
                    }
                }
                var account_new = entities.FirstOrDefault(x => x.Code == entity.Code && x.ParentRef == entity.ParentRef && x.WarehouseCode == warehouse.Code);
                if (account_new is null)
                {
                    account_new = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == entity.Code && x.ParentRef == entity.ParentRef && x.WarehouseCode == warehouse.Code);
                }

                if(account_new is not null)
                {
                    continue;
                }
                account_new = new ChartOfAccount();
                account_new.Code = entity.Code;
                account_new.Name = entity.Name;
                account_new.ParentRef = entity.ParentRef;
                account_new.Duration = entity.Duration;
                account_new.AccGroup = entity.AccGroup;
                account_new.Classification = entity.Classification;
                account_new.Protected = entity.Protected;
                account_new.WarehouseCode = warehouse.Code;
                account_new.WarehouseName = warehouse.Name;
                account_new.Type = entity.Type;
                account_new.StockUnit = entity.StockUnit;
                account_new.Year = year;

                _chartOfAccountAdds.Add(account_new);
            }
        }
    }
}
