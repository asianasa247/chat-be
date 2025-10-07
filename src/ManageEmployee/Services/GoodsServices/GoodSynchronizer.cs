using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.HotelEntities.RoomEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsServices
{
    public class GoodSynchronizer: IGoodSynchronizer
    {
        private readonly ApplicationDbContext _context;
        public GoodSynchronizer(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SyncAccountGood(int year)
        {
            try
            {
                var isNewGood = await CheckGoodNew(year);
                if (!isNewGood)
                    return;

                await _context.Database.BeginTransactionAsync();
                var listAccount = await _context.GetChartOfAccount(year)
                    .Where(x => x.Classification == 2 || x.Classification == 3)
                    .Select(x => new GoodChartOfAccountUpdateModel
                    {
                        Code = x.Code,
                        HasDetails = x.HasDetails,
                        Name = x.Name,
                        OpeningStockQuantityNB = x.OpeningStockQuantityNB,
                        ParentRef = x.ParentRef,
                        StockUnit = x.StockUnit,
                        Type = x.Type,
                        WarehouseCode = x.WarehouseCode,
                        WarehouseName = x.WarehouseName
                    })
                    .ToListAsync();
                await SetGoodFromAccountAsync(listAccount);

                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new ErrorException(ex.Message.ToString());
            }
        }

        public async Task SetGoodFromAccountAsync(List<GoodChartOfAccountUpdateModel> listAccount)
        {
            List<Goods> listGood_new = new List<Goods>();
            List<Goods> listGood_update = new List<Goods>();
            var listGood = _context.Goods.Where(x => x.PriceList == "BGC").ToList();

            var company = _context.Companies.OrderByDescending(x => x.SignDate).FirstOrDefault();

            foreach (var account in listAccount)
            {
                if (string.IsNullOrEmpty(account.ParentRef) || account.HasDetails || account.Type < 5)//
                    continue;
                string detail1 = account.Code;
                string detail2 = "";
                string code = account.ParentRef;
                if (account.ParentRef.Contains(":"))
                {
                    detail2 = account.Code;
                    string[] codes = account.ParentRef.Split(':');
                    code = codes[0];
                    detail1 = codes[1];
                }
                var goods = listGood.Find(x => x.Account == code
                            && x.Detail1 == detail1
                            && (string.IsNullOrEmpty(detail2) || x.Detail2 == detail2)
                            && (string.IsNullOrEmpty(account.WarehouseCode) || x.Warehouse == account.WarehouseCode));

                if (goods == null)
                {
                    goods = new Goods();
                    goods.PriceList = "BGC";
                    goods.Account = code;
                    string codeName = listAccount.Find(x => x.Code == code)?.Name;
                    if (string.IsNullOrEmpty(codeName))
                        continue;
                    goods.AccountName = codeName;
                    goods.Detail1 = detail1;
                    string detail1Name = listAccount.Find(x => x.Code == detail1 && x.ParentRef == code)?.Name;
                    goods.DetailName1 = detail1Name;

                    goods.Detail2 = detail2;
                    string detail2Name = "";
                    if (!string.IsNullOrEmpty(detail2))
                    {
                        detail2Name = listAccount.Find(x => x.Code == detail2 && x.ParentRef == code + ":" + detail1)?.Name;
                    }
                    goods.DetailName2 = detail2Name;

                    goods.Warehouse = account.WarehouseCode ?? "";
                    goods.WarehouseName = account.WarehouseName;
                    goods.Status = 1;
                    goods.Price = account.UnitPrice ?? 0;
                    goods.StockUnit = account.StockUnit;
                    goods.OpeningStockQuantityNB = account.OpeningStockQuantityNB;

                }

                if (account.UnitPrice != null && account.UnitPrice > 0)
                {
                    goods.SalePrice = account.UnitPrice ?? 0;
                }

                if (goods.Id != 0)
                {
                    listGood_update.Add(goods);
                }
                else
                {
                    listGood_new.Add(goods);
                }
            }
            
            await _context.Goods.AddRangeAsync(listGood_new);
            _context.Goods.UpdateRange(listGood_update);

            await _context.SaveChangesAsync();
            var roomTypes = listGood_new.Where(x => x.IsService).Select(x => new GoodRoomType
            {
                GoodId = x.Id,
            });

            await _context.GoodRoomTypes.AddRangeAsync(roomTypes);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckGoodNew(int year)
        {
            var listGood = await _context.Goods.Where(x => !x.IsDeleted && x.PriceList == "BGC" && !x.IsDeleted).ToListAsync();
            if (listGood.Count == 0)
                return true;

            var accountCodes = listGood.Select(x => x.Account).Distinct().ToList();

            var listAccount = await _context.GetChartOfAccount(year)
                .Where(x => (x.Classification == 2 || x.Classification == 3)
                && !string.IsNullOrEmpty(x.ParentRef) && x.Type > 4 && !x.HasDetails).ToListAsync();// 
            foreach (var account in listAccount)
            {
                string detail1 = account.Code;
                string detail2 = "";
                string code = account.ParentRef;

                if (account.ParentRef.Contains(":"))
                {
                    detail2 = account.Code;
                    string[] codes = account.ParentRef.Split(':');
                    code = codes[0];
                    detail1 = codes[1];
                }
                var goods = listGood.Find(x => x.Account == code && x.Detail1 == detail1
                && (string.IsNullOrEmpty(detail2) || x.Detail2 == detail2)
                && (string.IsNullOrEmpty(account.WarehouseCode) || x.Warehouse == account.WarehouseCode)
                );
                if (goods == null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}

public class GoodChartOfAccountUpdateModel
{
    public string ParentRef { get; set; }
    public string Code { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string StockUnit { get; set; }
    public double? OpeningStockQuantityNB { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public bool HasDetails { get; set; }
    public double? UnitPrice { get; set; }
}