using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsServices
{
    public class GoodsInWarehouseReporter : IGoodsInWarehouseReporter
    {
        private readonly ApplicationDbContext _context;

        public GoodsInWarehouseReporter(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagingResult<GoodsReportPositionModel>> ReportForGoodsInWarehouse(SearchViewModel param, int year)
        {
            try
            {
                if (param.PageSize <= 0)
                    param.PageSize = 20;

                if (param.Page < 0)
                    param.Page = 1;
                var results = ReportForGoodsInWarehouseQuery(param);

                if (!string.IsNullOrEmpty(param.Warehouse))
                {
                    results = results.Where(x => x.Warehouse == param.Warehouse);
                }
                if (!string.IsNullOrEmpty(param.GoodCode))
                {
                    results = results.Where(x => x.Detail1 == param.GoodCode || x.Detail2 == param.GoodCode);
                }
                if (param.MinStockType > 0)
                {
                    results = results.Where(x => (x.Quantity ?? 0 - x.MinStockLevel) < 0);
                }

                var goods = await results.OrderBy(x => x.Detail1).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize)
                    .Select(X => new GoodsReportPositionModel()
                    {
                        Account = X.Account,
                        AccountName = X.AccountName,
                        Detail1 = X.Detail1,
                        DetailName1 = X.DetailName1,
                        Detail2 = X.Detail2,
                        DetailName2 = X.DetailName2,
                        Quantity = X.Quantity
                    })
                    .ToListAsync();

                var warehouses = await _context.Warehouses.Where(x => !x.IsDelete).ToListAsync();
                var shevels = await _context.WareHouseShelves.ToListAsync();
                var floors = await _context.WareHouseFloors.ToListAsync();
                var positions = await _context.WareHousePositions.ToListAsync();

                var listStorege = await _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && !x.HasChild).ToListAsync();

                foreach (var good in goods)
                {
                    var goodWarehouses = await _context.GoodWarehouses.Where(x => x.Account == good.Account
                                && (string.IsNullOrEmpty(good.Detail1) || x.Detail1 == good.Detail1)
                                && (string.IsNullOrEmpty(good.Detail2) || x.Detail2 == good.Detail2)
                                ).ToListAsync();
                    if (!goodWarehouses.Any())
                        continue;
                    var goodWarehouseIds = goodWarehouses.Select(x => x.Id).ToList();
                    var goodWarehouseDetails = await _context.GoodWarehousesPositions.Where(x => goodWarehouseIds.Contains(x.GoodWarehousesId)).ToListAsync();
                    good.Positions = new List<string>();
                    foreach (var goodWarehouseDetail in goodWarehouseDetails)
                    {
                        var warehouse = warehouses.Find(X => X.Code == goodWarehouseDetail.Warehouse);
                        var shevel = shevels.Find(X => X.Id == goodWarehouseDetail.WareHouseShelvesId);
                        var floor = floors.Find(X => X.Id == goodWarehouseDetail.WareHouseFloorId);
                        var position = positions.Find(X => X.Id == goodWarehouseDetail.WareHousePositionId);
                        good.Positions.Add("Số lượng " + goodWarehouseDetail.Quantity.ToString() + " " + warehouse?.Name + ", " + shevel?.Name + ", " + floor?.Name + ", " + position?.Name);
                    }

                    ChartOfAccount storege;
                    if (!string.IsNullOrEmpty(good.Detail2))
                    {
                        string parentRef = good.Account + ":" + good.Detail1;
                        storege = listStorege.Find(x => x.Code == good.Detail2 && x.ParentRef == parentRef &&
                                (string.IsNullOrEmpty(good.Warehouse) || x.WarehouseCode == good.Warehouse));
                    }
                    else if (!string.IsNullOrEmpty(good.Detail1))
                        storege = listStorege.Find(x => x.Code == good.Detail1 && x.ParentRef == good.Account &&
                        (string.IsNullOrEmpty(good.Warehouse) || x.WarehouseCode == good.Warehouse));
                    else
                        storege = listStorege.Find(x => x.Code == good.Account);

                    if (storege != null)
                    {
                        good.Quantity = (storege.OpeningStockQuantity ?? 0) + (storege.ArisingStockQuantity ?? 0);
                        good.StockUnit = storege.StockUnit;
                    }
                    if (good.Quantity == null || good.Quantity < 0)
                        good.Quantity = 0;
                }
                return new PagingResult<GoodsReportPositionModel>()
                {
                    CurrentPage = param.Page,
                    PageSize = param.PageSize,
                    TotalItems = results.Count(),
                    Data = goods
                };
            }
            catch
            {
                return new PagingResult<GoodsReportPositionModel>()
                {
                    CurrentPage = param.Page,
                    PageSize = param.PageSize,
                    TotalItems = 0,
                    Data = new List<GoodsReportPositionModel>()
                };
            }
        }

        private IQueryable<GoodsExportlModel> ReportForGoodsInWarehouseQuery(SearchViewModel param)
        {
            var results = from p in _context.Goods
                          where !p.IsDeleted
                          && (string.IsNullOrEmpty(param.GoodType) || p.GoodsType == param.GoodType)
                          && (string.IsNullOrEmpty(param.Account) || p.Account == param.Account)
                          && (string.IsNullOrEmpty(param.Detail1) || p.Detail1 == param.Detail1)
                          && (string.IsNullOrEmpty(param.PriceCode) || p.PriceList == param.PriceCode)
                          && (string.IsNullOrEmpty(param.MenuType) || p.MenuType == param.MenuType)
                          && (string.IsNullOrEmpty(param.Position) || p.Position == param.Position)
                          && (string.IsNullOrEmpty(param.SearchText) || (!string.IsNullOrEmpty(p.Detail2) ? p.DetailName2 : p.DetailName1 ?? p.AccountName).Contains(param.SearchText)
                          || p.SalePrice.ToString().Contains(param.SearchText) || p.Detail2.Contains(param.SearchText))

                          && p.Status == param.Status
                          select new GoodsExportlModel()
                          {
                              Id = p.Id,
                              MenuType = p.MenuType,
                              Account = p.Account,
                              AccountName = p.AccountName,
                              Delivery = p.Delivery,
                              Warehouse = p.Warehouse,
                              WarehouseName = p.WarehouseName,
                              Detail1 = p.Detail1,
                              Detail2 = p.Detail2,
                              DetailName1 = p.DetailName1,
                              DetailName2 = p.DetailName2,
                              GoodsType = p.GoodsType,
                              Inventory = p.Inventory,
                              IsDeleted = p.IsDeleted,
                              MaxStockLevel = p.MaxStockLevel,
                              MinStockLevel = p.MinStockLevel,
                              Position = p.Position,
                              Price = p.Price,
                              SalePrice = p.SalePrice,
                              DiscountPrice = p.DiscountPrice,
                              PriceList = p.PriceList,
                              isPromotion = p.isPromotion,
                          };

            return results;
        }

    }
}
