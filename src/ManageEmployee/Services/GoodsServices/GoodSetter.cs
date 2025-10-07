using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.HotelEntities.RoomEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsServices
{
    public class GoodSetter: IGoodSetter
    {
        private readonly ApplicationDbContext _context;

        public GoodSetter(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Create(GoodsUpdateModel param, int year)
        {

            await ValidateWhenSetting(param.Account, param.Detail1, param.Detail2, param.Warehouse, param.Id);

            var accountCode = param.Account;
            string parentRef = "";
            if (!string.IsNullOrEmpty(param.Detail2))
            {
                accountCode = param.Detail2;
                parentRef = param.Account + ":" + param.Detail1;
            }
            else if (!string.IsNullOrEmpty(param.Detail1))
            {
                accountCode = param.Detail1;
                parentRef = param.Account;
            }
            var chartofAccount = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == accountCode && x.ParentRef == parentRef
                               && (string.IsNullOrEmpty(param.Warehouse) || x.WarehouseCode == param.Warehouse));
            if (chartofAccount != null)
            {
                chartofAccount.StockUnitPriceNB = param.StockUnitPriceNB;
                chartofAccount.OpeningDebitNB = param.OpeningDebitNB;
                chartofAccount.OpeningStockQuantityNB = param.OpeningStockQuantityNB;
                param.StockUnit = chartofAccount.StockUnit;

                _context.ChartOfAccounts.Update(chartofAccount);
            }

            await _context.Goods.AddAsync(param);


            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> Update(GoodsUpdateModel param, int year)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var goods = _context.Goods.SingleOrDefault(x => x.Id == param.Id && !x.IsDeleted);
                if (goods == null)
                {
                    return ErrorMessages.DataNotFound;
                }
                await ValidateWhenSetting(param.Account, param.Detail1, param.Detail2, param.Warehouse, param.Id);

                goods.Account = param.Account;
                goods.AccountName = param.AccountName;
                goods.Delivery = param.Delivery;
                goods.PriceList = param.PriceList;
                goods.GoodsType = param.GoodsType;
                goods.Inventory = param.Inventory;
                goods.MaxStockLevel = param.MaxStockLevel;
                goods.MenuType = param.MenuType;
                goods.MinStockLevel = param.MinStockLevel;
                goods.Position = param.Position;
                goods.Price = param.Price;
                goods.SalePrice = param.SalePrice;
                goods.DiscountPrice = param.DiscountPrice;
                goods.Warehouse = param.Warehouse;
                goods.WarehouseName = param.WarehouseName;
                goods.Detail1 = param.Detail1;
                goods.Detail2 = param.Detail2;
                goods.DetailName1 = param.DetailName1;
                goods.DetailName2 = param.DetailName2;
                goods.Status = param.Status;
                goods.TaxRateId = param.TaxRateId;
                goods.Net = param.Net;
                goods.IsService = param.IsService;

                if (param.Image1 == "" && !File.Exists(goods.Image1) && !string.IsNullOrEmpty(goods.Image1))
                {
                    goods.Image1 = "";
                }
                else
                {
                    goods.Image1 = param.Image1;
                }

                if (param.Image2 == "" && !File.Exists(goods.Image2) && !string.IsNullOrEmpty(goods.Image2))
                {
                    goods.Image2 = "";
                }
                else
                {
                    goods.Image2 = param.Image2;
                }

                if (param.Image3 == "" && !File.Exists(goods.Image3) && !string.IsNullOrEmpty(goods.Image3))
                {
                    goods.Image3 = "";
                }
                else
                {
                    goods.Image3 = param.Image3;
                }

                if (param.Image4 == "" && !File.Exists(goods.Image4) && !string.IsNullOrEmpty(goods.Image4))
                {
                    goods.Image4 = "";
                }
                else
                {
                    goods.Image4 = param.Image4;
                }

                if (param.Image5 == "" && !File.Exists(goods.Image5) && !string.IsNullOrEmpty(goods.Image5))
                {
                    goods.Image5 = "";
                }
                else
                {
                    goods.Image5 = param.Image5;
                }

                _context.Goods.Update(goods);

                var accountCode = goods.Account;
                string parentRef = "";
                if (!string.IsNullOrEmpty(goods.Detail2))
                {
                    accountCode = goods.Detail2;
                    parentRef = goods.Account + ":" + goods.Detail1;
                }
                else if (!string.IsNullOrEmpty(goods.Detail1))
                {
                    accountCode = goods.Detail1;
                    parentRef = goods.Account;
                }
                var chartofAccount = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == accountCode && x.ParentRef == parentRef
                                && (string.IsNullOrEmpty(goods.Warehouse) || x.WarehouseCode == goods.Warehouse));
                if (chartofAccount != null)
                {
                    chartofAccount.StockUnitPriceNB = param.StockUnitPriceNB;
                    chartofAccount.OpeningDebitNB = param.OpeningDebitNB;
                    chartofAccount.OpeningStockQuantityNB = param.OpeningStockQuantityNB;
                    _context.ChartOfAccounts.Update(chartofAccount);
                }
                var roomTypes = await _context.GoodRoomTypes.FirstOrDefaultAsync(x => x.GoodId == param.Id);
                if (!param.IsService && roomTypes is not null)
                {
                    _context.GoodRoomTypes.Remove(roomTypes);
                }
                if (param.IsService && roomTypes is null)
                {
                    var roomTypeAdds = new GoodRoomType
                    {
                        GoodId = param.Id,
                    };
                    await _context.GoodRoomTypes.AddAsync(roomTypeAdds);
                }

                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        public async Task Delete(int id)
        {
            var goods = await _context.Goods.FindAsync(id);
            if (goods != null)
            {
                // check
                goods.IsDeleted = true;
                _context.Goods.Update(goods);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> UpdateGoodsWebsite(Goods requests)
        {
            try
            {
                var goods = _context.Goods.SingleOrDefault(x => x.Id == requests.Id && !x.IsDeleted);
                if (goods == null)
                {
                    return ErrorMessages.DataNotFound;
                }
                await ValidateWhenSetting(requests.Account, requests.Detail1, requests.Detail2, requests.Warehouse, requests.Id);

                goods.TitleVietNam = requests.TitleVietNam;
                goods.WebDiscountVietNam = requests.WebDiscountVietNam;
                goods.WebGoodNameVietNam = requests.WebGoodNameVietNam;
                goods.WebPriceVietNam = requests.WebPriceVietNam;
                goods.ContentVietNam = requests.ContentVietNam;

                goods.TitleKorea = requests.TitleKorea;
                goods.WebDiscountKorea = requests.WebDiscountKorea;
                goods.WebGoodNameKorea = requests.WebGoodNameKorea;
                goods.WebPriceKorea = requests.WebPriceKorea;
                goods.ContentKorea = requests.ContentKorea;

                goods.TitleEnglish = requests.TitleEnglish;
                goods.WebDiscountEnglish = requests.WebDiscountEnglish;
                goods.WebGoodNameEnglish = requests.WebGoodNameEnglish;
                goods.WebPriceEnglish = requests.WebPriceEnglish;
                goods.ContentEnglish = requests.ContentEnglish;
                goods.MenuType = requests.MenuType;
                if (requests.Image1 == "" && !File.Exists(goods.Image1) && !string.IsNullOrEmpty(goods.Image1))
                {
                    goods.Image1 = "";
                }
                else
                {
                    goods.Image1 = requests.Image1;
                }

                if (requests.Image2 == "" && !File.Exists(goods.Image2) && !string.IsNullOrEmpty(goods.Image2))
                {
                    goods.Image2 = "";
                }
                else
                {
                    goods.Image2 = requests.Image2;
                }

                if (requests.Image3 == "" && !File.Exists(goods.Image3) && !string.IsNullOrEmpty(goods.Image3))
                {
                    goods.Image3 = "";
                }
                else
                {
                    goods.Image3 = requests.Image3;
                }

                if (requests.Image4 == "" && !File.Exists(goods.Image4) && !string.IsNullOrEmpty(goods.Image4))
                {
                    goods.Image4 = "";
                }
                else
                {
                    goods.Image4 = requests.Image4;
                }

                if (requests.Image5 == "" && !File.Exists(goods.Image5) && !string.IsNullOrEmpty(goods.Image5))
                {
                    goods.Image5 = "";
                }
                else
                {
                    goods.Image5 = requests.Image5;
                }

                _context.Goods.Update(goods);
                await _context.SaveChangesAsync();
                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        private async Task ValidateWhenSetting(string account, string detail1, string detail2, string wareHouse, int goodId)
        {
            var shoudIgnorSetting = await _context.Goods.AnyAsync(x => x.Id != goodId
                                                            && x.Account == account
                                                            && !string.IsNullOrEmpty(x.Detail1) && !string.IsNullOrEmpty(detail1) && x.Detail1 == detail1
                                                            && !string.IsNullOrEmpty(x.Detail2) && !string.IsNullOrEmpty(detail2) && x.Detail2 == detail2
                                                            && !string.IsNullOrEmpty(x.Warehouse) && !string.IsNullOrEmpty(wareHouse) && x.Warehouse == wareHouse
                                                            );
            if (shoudIgnorSetting)
            {
                throw new ErrorException(ErrorMessages.GoodsCodeAlreadyExist);
            }
        }

        public async Task<bool> CheckExistGoods(Goods requests)
        {
            var exist = await _context.Goods.SingleOrDefaultAsync(
                    x => !x.IsDeleted && x.Detail1 == requests.Detail1 && x.Detail2 == requests.Detail2
                    && x.DetailName1 == requests.DetailName1 && x.DetailName2 == requests.DetailName2
                    && x.Warehouse == requests.Warehouse && x.WarehouseName == requests.WarehouseName
                    && x.PriceList == requests.PriceList);
            return exist != null && exist.Id != requests.Id;
        }


        public async Task UpdateMenuTypeForGood(UpdateMenuTypeForGoodModel request)
        {
            if (request is null)
            {
                throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
            }

            if (request.MenuType == null)
            {
                throw new ErrorException(ErrorMessage.USERID_IS_EMPTY);
            }

            if (request.GoodIds == null)
            {
                throw new ErrorException(ErrorMessage.USERID_IS_EMPTY);
            }

            var isExistMenuType = await _context.Categories.AnyAsync(x => x.Code == request.MenuType && (x.Type == (int)CategoryEnum.MenuWeb || x.Type == (int)CategoryEnum.MenuWebOnePage));
            if (!isExistMenuType)
            {
                throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
            }

            var goods = await _context.Goods.Where(x => request.GoodIds.Contains(x.Id)).ToListAsync();
            goods = goods.ConvertAll(x => { x.MenuType = request.MenuType; return x; });
            _context.Goods.UpdateRange(goods);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateStatusGoods(List<int> goodIds, int status)
        {
            if (status != 0 && status != 1)
            {
                throw new ErrorException("Not exist status");
            }

            var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
            if (!goods.Any())
            {
                throw new ErrorException("Not exist goods");
            }
            goods = goods.ConvertAll(x =>
            {
                x.Status = status;
                return x;
            });
            _context.Goods.UpdateRange(goods);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGoodIsService(List<int> goodIds)
        {
            var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
            if (!goods.Any())
            {
                throw new ErrorException("Not exist goods");
            }
            goods = goods.ConvertAll(x =>
            {
                x.IsService = true;
                return x;
            });
            _context.Goods.UpdateRange(goods);

            var roomTypeGoodIds = await _context.GoodRoomTypes.Where(x => goodIds.Contains(x.GoodId)).Select(x => x.GoodId).ToListAsync();

            var roomTypeAdds = goodIds.Where(x => !roomTypeGoodIds.Contains(x)).Select(x => new GoodRoomType
            {
                GoodId = x,
            });
            await _context.GoodRoomTypes.AddRangeAsync(roomTypeAdds);
            await _context.SaveChangesAsync();
        }
    }
}
