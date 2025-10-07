using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsServices
{
    public class GoodImporter: IGoodImporter
    {
        private readonly ApplicationDbContext _context;

        public GoodImporter(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ImportFromExcel(List<GoodsExportlModel> lstGoods, bool isManager)
        {
            if (!isManager)
            {
                await ImportFromExcelForPriceList(lstGoods);
            }

            if (lstGoods.Count == 0)
                throw new ErrorException(ErrorMessages.DataNotFound);

            using var trans = await _context.Database.BeginTransactionAsync();

            List<Goods> goodChecks = await _context.Goods.Where(x => !x.IsDeleted && x.PriceList == "BGC").ToListAsync();
            var company = await _context.Companies.OrderByDescending(x => x.SignDate).FirstOrDefaultAsync();
            var taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
            taxRates = taxRates.Where(x => x.Code.StartsWith("R")).ToList();

            foreach (var goods in lstGoods)
            {
                Goods item = goodChecks.Find(x => x.Account == goods.Account && x.Detail1 == goods.Detail1
                                                    && ((string.IsNullOrEmpty(x.Detail2) && string.IsNullOrEmpty(goods.Detail2)) || x.Detail2 == goods.Detail2)
                                                    && (x.Warehouse == goods.Warehouse || string.IsNullOrEmpty(goods.Warehouse))
                                                    && x.Status == 1);
                if (item == null)
                    item = new Goods();

                item.Account = goods.Account;
                item.AccountName = goods.AccountName;
                item.Detail1 = goods.Detail1;
                item.DetailName1 = goods.DetailName1;
                item.Detail2 = goods.Detail2;
                item.DetailName2 = goods.DetailName2;
                item.Warehouse = goods.Warehouse;
                item.WarehouseName = goods.WarehouseName;
                item.GoodsType = goods.GoodsType;
                item.MinStockLevel = goods.MinStockLevel;
                item.MaxStockLevel = goods.MaxStockLevel;
                item.Net = goods.Net;
                item.SalePrice = goods.SalePrice;
                item.WebPriceVietNam = goods.WebPriceVietNam;
                item.Status = 1;
                var taxRateName = goods.TaxRateName.Split("-");
                if (taxRateName.Length > 0)
                {
                    var taxRate = taxRates.Find(x => x.Name == taxRateName[0]);
                    item.TaxRateId = taxRate?.Id;
                }

                item.PriceList = "BGC";
                if (item.Id > 0)
                    _context.Goods.Update(item);
                else
                {
                    _context.Goods.Add(item);
                }
            }

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }

        private async Task ImportFromExcelForPriceList(List<GoodsExportlModel> lstGoods)
        {
            if (lstGoods.Count == 0)
                throw new ErrorException(ErrorMessages.DataNotFound);

            using var trans = await _context.Database.BeginTransactionAsync();

            List<Category> lstGroupType = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            var taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
            taxRates = taxRates.Where(x => x.Code.StartsWith("R")).ToList();

            var listMenuType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.GoodGroup).ToList();
            var listGoodsType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.GoodsType2).ToList();
            var listPositionType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.Position).ToList();

            var company = await _context.Companies.OrderByDescending(x => x.SignDate).FirstOrDefaultAsync();
            List<Goods> goodUpdates = new List<Goods>();
            List<Goods> goodAdds = new List<Goods>();
            foreach (var goods in lstGoods)
            {
                Goods item = await _context.Goods.FirstOrDefaultAsync(x => x.Account == goods.Account
                                                    && x.Detail1 == goods.Detail1
                                                    && (x.Detail2 == goods.Detail2 || string.IsNullOrEmpty(goods.Detail2))
                                                    && (x.Warehouse == goods.Warehouse || string.IsNullOrEmpty(goods.Warehouse))
                                                    && x.Status == goods.Status
                                                    && x.PriceList == goods.PriceList
                                                    && !x.IsDeleted);
                if (item == null)
                    item = new Goods();
                item.Account = goods.Account;
                item.AccountName = goods.AccountName;
                item.Detail1 = goods.Detail1;
                item.DetailName1 = goods.DetailName1;
                item.Detail2 = goods.Detail2;
                item.DetailName2 = goods.DetailName2;
                item.Warehouse = goods.Warehouse;
                item.WarehouseName = goods.WarehouseName;
                item.DiscountPrice = goods.DiscountPrice;
                item.Image1 = goods.Image1;
                item.Image2 = goods.Image2;
                item.Image3 = goods.Image3;
                item.Image4 = goods.Image4;
                item.Image5 = goods.Image5;
                item.Price = goods.Price;
                item.PriceList = goods.PriceList;
                item.SalePrice = goods.SalePrice;
                if (goods.TaxRateName != null)
                {
                    var taxRateName = goods.TaxRateName.Split("-");
                    if (taxRateName.Length > 0)
                    {
                        var taxRate = taxRates.Find(x => x.Name == taxRateName[0]);
                        item.TaxRateId = taxRate?.Id;
                    }
                }
                item.MenuType = listMenuType.Find(t => t.Name == goods.MenuType)?.Code;
                item.GoodsType = listGoodsType.Find(t => t.Name == item.GoodsType)?.Code;
                item.Position = listPositionType.Find(t => t.Name == item.Position)?.Code;

                if (item.Id > 0)
                {
                    if (!goodUpdates.Exists(x => x.Id == item.Id))
                        goodUpdates.Add(item);
                }
                else
                {
                    goodAdds.Add(item);
                }
            }
            _context.Goods.UpdateRange(goodUpdates);
            await _context.Goods.AddRangeAsync(goodAdds);

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }
    }
}
