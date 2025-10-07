using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Services.Interfaces.Inventorys;

namespace ManageEmployee.Services;
public class InventoryService: IInventoryService
{
    private readonly ApplicationDbContext _context;
    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<Inventory> GetListData(PagingRequestModel param, int year)
    {
        try
        {
            if (param.Page <= 0)
                param.Page = 1;
            if (param.PageSize <= 0)
                param.PageSize = 25;
            List<Inventory> goods = _context.Goods.Where(x => x.Status == 1 && x.PriceList == "BGC")
                  .Select(k => new Inventory
                  { 
                      Account = k.Account,
                      AccountName = k.AccountName,
                      Detail1 = k.Detail1,
                      DetailName1 = k.DetailName1,
                      Detail2 = k.Detail2,
                      DetailName2 = k.DetailName2,
                      Warehouse = k.Warehouse,
                      WarehouseName = k.WarehouseName,
                      CreateAt = DateTime.Today,
                      Image1 = k.Image1
                  }).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize)
                  .ToList();
            List<string> accountCodes = goods.Select(x => x.Account).Distinct().ToList();

            List<SoChiTietViewModel> relations = _context.GetLedger(year)
                .Where(x => accountCodes.Contains(x.DebitCode)|| accountCodes.Contains(x.CreditCode))
            .Select(k => new SoChiTietViewModel
            {
                DebitCode = k.DebitCode,
                CreditCode = k.CreditCode,
                Description = k.OrginalDescription,
                Quantity = k.Quantity,
                DebitDetailCodeFirst = k.DebitDetailCodeFirst,
                DebitDetailCodeSecond = k.DebitDetailCodeSecond,
                CreditDetailCodeFirst = k.CreditDetailCodeFirst,
                CreditDetailCodeSecond = k.CreditDetailCodeSecond,
            })
            .ToList();
            var listAccount = _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && x.DisplayInsert == true).ToList();
            foreach(var good in goods)
            {
                good.InputQuantity = relations.Where(x => x.CreditCode == good.Account && x.CreditDetailCodeFirst == good.Detail1 && x.CreditDetailCodeSecond == good.Detail2).Sum(x => x.Quantity);
                good.OutputQuantity = relations.Where(x => x.DebitCode == good.Account && x.DebitDetailCodeFirst == good.Detail1 && x.DebitDetailCodeSecond == good.Detail2).Sum(x => x.Quantity);
                good.CloseQuantity = (listAccount.FirstOrDefault(x => !string.IsNullOrEmpty(good.Detail2) ? (x.Code == good.Detail2 && x.ParentRef==(good.Account+":"+good.Detail1)) : 
                                                                    (!string.IsNullOrEmpty(good.Detail1) ? (x.Code == good.Detail1 && x.ParentRef == good.Account) :x.Code == good.Account)).OpeningStockQuantity ?? 0) + good.InputQuantity - good.OutputQuantity;
                
            }
            return goods;
        }
        catch
        {
            return null;
        }
    }

    public string Create(List<Inventory> datas)
    {
        foreach(var data in datas)
        {
            if (data.Id > 0)
                _context.Inventory.Update(data);
            else
                _context.Inventory.Add(data);
        }
        _context.SaveChanges();
        return "";
    }
    public IEnumerable<Inventory> GetListInventory(InventoryRequestModel param)
    {
        if(param.dtMax == null)
            param.dtMax = _context.Inventory.Select(x => x.CreateAt).OrderByDescending(x => x).FirstOrDefault();
        return _context.Inventory.Where(x => x.CreateAt == param.dtMax);
    }
    public List<DateTime> GetListDateInventory()
    {
        return _context.Inventory.OrderByDescending(x => x.CreateAt).Select(x => x.CreateAt).Distinct().ToList();
    }
    public string Accept(List<Inventory> datas)
    {
        foreach (var data in datas)
        {
            data.isCheck = true;
            var goodWareHouse = _context.GoodWarehouses.FirstOrDefault(x => x.Account == data.Account
                                    && (string.IsNullOrEmpty(data.Detail1) || x.Detail1 == data.Detail1)
                                    && (string.IsNullOrEmpty(data.Detail2) || x.Detail2 == data.Detail2)
                                    && (string.IsNullOrEmpty(data.Warehouse) || x.Warehouse == data.Warehouse)
                                    && x.DateExpiration == data.DateExpiration);
            if(goodWareHouse == null)
            {
                goodWareHouse = new GoodWarehouses();
                goodWareHouse.Account = data.Account;
                goodWareHouse.AccountName = data.AccountName;
                goodWareHouse.Detail1 = data.Detail1;
                goodWareHouse.DetailName1 = data.DetailName1;
                goodWareHouse.Detail2 = data.Detail2;
                goodWareHouse.DetailName2 = data.DetailName2;
                goodWareHouse.Warehouse = data.Warehouse;
                goodWareHouse.WarehouseName = data.WarehouseName;
                goodWareHouse.Quantity = data.CloseQuantityReal;
                goodWareHouse.Order = 0;
                goodWareHouse.Status = 1;
                goodWareHouse.DateExpiration = data.DateExpiration;
                _context.GoodWarehouses.Add(goodWareHouse);
            }
        }
        _context.SaveChanges();
        return "";
    }
}
