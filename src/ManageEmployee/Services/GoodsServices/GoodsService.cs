using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.ConvertProductEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using ManageEmployee.ViewModels;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Threading;
using Xceed.Document.NET;

namespace ManageEmployee.Services.GoodsServices;

public class GoodsService : IGoodsService
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;
    private readonly IMapper _mapper;
    private readonly ILedgerService _ledgerService;

    public GoodsService(ApplicationDbContext context,
        IChartOfAccountV2Service chartOfAccountV2Service,
        IMapper mapper,
        ILedgerService ledgerService)
    {
        _context = context;
        _chartOfAccountV2Service = chartOfAccountV2Service;
        _mapper = mapper;
        _ledgerService = ledgerService;
    }

    public async Task<IEnumerable<Goods>> GetAll(Expression<Func<Goods, bool>> where, int pageSize = 10)
    {
        return await _context.Goods.Where(x => !x.IsDeleted).Where(where).OrderByDescending(x => x.DiscountPrice).Take(pageSize).ToListAsync();
    }

    public async Task<GoodsPagingResult> GetPaging(SearchViewModel param, int year)
    {
        try
        {

            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;
            var results = GetAll_Common(param);

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
                results = results.Where(x => (x.Quantity ?? 0) - x.MinStockLevel < 0);
            }

            var searchTexts = new List<string>();
            if (!string.IsNullOrEmpty(param.SearchText))
            {
                searchTexts = param.SearchText.Split("&").Select(x => x.ToLower()).ToList();
            }

            if(param is ISearchWithGoodsIds searchWithGoodsIds && searchWithGoodsIds.GoodsIds != null && searchWithGoodsIds.GoodsIds.Any())
            {
                results = results.Where(x => searchWithGoodsIds.GoodsIds.Contains(x.Id));
            }

            List<GoodsExportlModel> goodDatas = await results.ToListAsync();

            if (searchTexts.Any())
            {
                var convertItem = _context.ConvertProducts.ToList()
                                     .FirstOrDefault(x => searchTexts.All(s => x.OppositeDetail1.ToLower().Contains(s))
                                                            || searchTexts.All(s => x.Detail1.ToLower().Contains(s))
                                                            || searchTexts.All(s => x.OppositeDetail2.ToLower().Contains(s))
                                                            || searchTexts.All(s => x.Detail2.ToLower().Contains(s))
                                                            );
                if (convertItem != null)
                {
                    goodDatas = goodDatas.Where(p => (!string.IsNullOrEmpty(p.Detail2) 
                        && p.Detail2 == convertItem.Detail2 || p.Detail2 == convertItem.OppositeDetail2)
                                  && (!string.IsNullOrEmpty(p.Detail1)
                             && p.Detail1 == convertItem.Detail1 || p.Detail1 == convertItem.OppositeDetail1)
                             ).ToList();
                }
                else
                {
                    goodDatas = goodDatas.Where(p => (!string.IsNullOrEmpty(p.Detail2) && searchTexts.All(s => p.Detail2.ToLower().Contains(s)))
                              || (!string.IsNullOrEmpty(p.DetailName2) && searchTexts.All(s => p.DetailName2.ToLower().Contains(s)))
                             || (!string.IsNullOrEmpty(p.Detail1) && searchTexts.All(s => p.Detail1.ToLower().Contains(s)))
                             || (!string.IsNullOrEmpty(p.DetailName1) && searchTexts.All(s => p.DetailName1.ToLower().Contains(s)))
                             ).ToList();

                }

            }
            //storege
            if (param.isCashier || param.isManage)
            {
                var listStorege = await _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && !x.HasChild).ToListAsync();
                foreach (var item in goodDatas)
                {
                    ChartOfAccount storege;
                    if (!string.IsNullOrEmpty(item.Detail2))
                    {
                        string parentRef = item.Account + ":" + item.Detail1;
                        storege = listStorege.Find(x => x.Code == item.Detail2 && x.ParentRef == parentRef &&
                                (string.IsNullOrEmpty(item.Warehouse) || x.WarehouseCode == item.Warehouse));
                    }
                    else if (!string.IsNullOrEmpty(item.Detail1))
                        storege = listStorege.Find(x => x.Code == item.Detail1 && x.ParentRef == item.Account &&
                        (string.IsNullOrEmpty(item.Warehouse) || x.WarehouseCode == item.Warehouse));
                    else
                        storege = listStorege.Find(x => x.Code == item.Account);

                    if (storege != null)
                    {
                        item.Quantity = (storege.OpeningStockQuantityNB ?? 0) + (storege.ArisingStockQuantityNB ?? 0);
                        item.StockUnit = storege.StockUnit;
                        item.OpeningStockQuantityNB = storege.OpeningStockQuantityNB;
                    }
                }

                if (param.isQuantityStock)
                    goodDatas = goodDatas.Where(x => x.Quantity > 0).ToList();
            }
            if (param.isCashier)
            {
                goodDatas = await GetGoodConvert(goodDatas);
            }

            var goods = param.Page == 0 ? goodDatas.OrderBy(x => x.Detail1).ToList() : goodDatas.OrderBy(x => x.Detail1).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToList();

            var company = await _context.Companies.FirstOrDefaultAsync();
            var goodsQuotaIds = goods.Select(x => x.GoodsQuotaId);
            var goodsQuotas = await _context.GoodsQuotas.Where(x => goodsQuotaIds.Contains(x.Id)).ToListAsync();

            foreach (var item in goods)
            {
                if (param.isCashier || param.isManage)
                {
                    item.QrCodes = await _context.GoodWarehouses.Where(x =>
                                        (string.IsNullOrEmpty(x.Account) || x.Account == item.Account) &&
                                        (string.IsNullOrEmpty(item.Detail1) || x.Detail1 == item.Detail1) &&
                                        (string.IsNullOrEmpty(item.Detail2) || x.Detail2 == item.Detail2) &&
                                        (string.IsNullOrEmpty(item.Warehouse) || x.Warehouse == item.Warehouse))
                                    .Select(x => (!string.IsNullOrEmpty(x.Detail2) ? x.Detail2 : x.Detail1 ?? x.Account) + " " + x.Order + "-" + x.Id)
                                    .ToListAsync();
                    if (string.IsNullOrEmpty(item.Image1))
                    {
                        item.Image1 = company?.FileLogo;
                    }
                }
                item.GoodsQuotaName = goodsQuotas.FirstOrDefault(X => X.Id == item.GoodsQuotaId)?.Code;

                if (!string.IsNullOrEmpty(param.GoodType))
                {
                    var goodDetails = await _context.GoodDetails.Where(x => !(x.IsDeleted ?? false) && x.GoodID == item.Id).ToListAsync();
                    item.ListDetailName = string.Join("; ", goodDetails.Select(x => !string.IsNullOrEmpty(x.Detail2) ? x.Detail2 : x.Detail1 ?? x.Account).ToArray());
                    item.TotalAmount = goodDetails.Sum(x => x.Amount);
                }
            }

            // Sort
            goods = goods.OrderBy(x => (x.Quantity ?? 0) - x.MinStockLevel).ToList();

            return new GoodsPagingResult()
            {
                pageIndex = param.Page,
                PageSize = param.PageSize,
                TotalItems = goodDatas.Count,
                Goods = goods
            };
        }
        catch
        {
            return new GoodsPagingResult()
            {
                pageIndex = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Goods = new List<Goods>()
            };
        }
    }

    public async Task<GoodslDetailModel> GetById(int id, int year)
    {
        var goods = await _context.Goods.FindAsync(id);
        var item = _mapper.Map<GoodslDetailModel>(goods);
        if (!string.IsNullOrEmpty(goods.Account))
        {
            item.AccountObj = await _chartOfAccountV2Service.FindAccount(goods.Account, string.Empty, year);
            if (!string.IsNullOrEmpty(goods.Detail1))
            {
                item.DetailFirstObj = await _chartOfAccountV2Service.FindAccount(goods.Detail1, goods.Account, year);
                if (!string.IsNullOrEmpty(goods.Detail2))
                {
                    item.DetailSecondObj = await _chartOfAccountV2Service.FindAccount(goods.Detail2, goods.Account + ":" + goods.Detail1, year);
                }
            }
        }

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
            item.StockUnitPriceNB = chartofAccount.StockUnitPriceNB;
            item.OpeningDebitNB = chartofAccount.OpeningDebitNB;
            if (string.IsNullOrEmpty(item.WebGoodNameVietNam))
                item.WebGoodNameVietNam = chartofAccount.Name;
        }

        if (item.WebPriceVietNam == null || item.WebPriceVietNam == 0)
            item.WebPriceVietNam = item.SalePrice;

        return item;
    }

    public IQueryable<GoodsExportlModel> GetAll_Common(SearchViewModel param)
    {
        var results = from p in _context.Goods
                      join t in _context.TaxRates on p.TaxRateId equals t.Id into _t
                      from t in _t.DefaultIfEmpty()

                      where !p.IsDeleted
                      && (string.IsNullOrEmpty(param.GoodType) || p.GoodsType == param.GoodType)
                      && (string.IsNullOrEmpty(param.Account) || p.Account == param.Account)
                      && (string.IsNullOrEmpty(param.Detail1) || p.Detail1 == param.Detail1)
                      && (string.IsNullOrEmpty(param.PriceCode) || p.PriceList == param.PriceCode)
                      && (string.IsNullOrEmpty(param.MenuType) || p.MenuType == param.MenuType)
                      && (string.IsNullOrEmpty(param.Position) || p.Position == param.Position)

                      //&& (string.IsNullOrEmpty(param.SearchText)
                      //         || p.Detail2.Contains(param.SearchText)
                      //         || p.DetailName2.Contains(param.SearchText)
                      //         || p.Detail1.Contains(param.SearchText)
                      //         || p.DetailName1.Contains(param.SearchText))

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
                          Detail1English = p.Detail1English,
                          DetailName1English = p.DetailName1English,
                          Detail1Korean = p.Detail1Korean,
                          DetailName1Korean = p.DetailName1Korean,
                          GoodsType = p.GoodsType,
                          Image1 = p.Image1,
                          Image2 = p.Image2,
                          Image3 = p.Image3,
                          Image4 = p.Image4,
                          Image5 = p.Image5,
                          Inventory = p.Inventory,
                          IsDeleted = p.IsDeleted,
                          MaxStockLevel = p.MaxStockLevel,
                          MinStockLevel = p.MinStockLevel,
                          Position = p.Position,
                          Price = p.Price,
                          SalePrice = p.SalePrice,
                          DiscountPrice = p.DiscountPrice,
                          Status = p.Status,
                          PriceList = p.PriceList,

                          ContentEnglish = !param.isCashier && !param.isManage ? p.ContentEnglish : "",
                          ContentKorea = !param.isCashier && !param.isManage ? p.ContentKorea : "",
                          ContentVietNam = !param.isCashier && !param.isManage ? p.ContentVietNam : "",
                          TitleEnglish = !param.isCashier && !param.isManage ? p.TitleEnglish : "",
                          TitleKorea = !param.isCashier && !param.isManage ? p.TitleKorea : "",
                          TitleVietNam = !param.isCashier && !param.isManage ? p.TitleVietNam : "",

                          //WebDiscountEnglish = p.WebDiscountEnglish,
                          //WebDiscountKorea = p.WebDiscountKorea,
                          //WebDiscountVietNam = p.WebDiscountVietNam,
                          //WebGoodNameEnglish = p.WebGoodNameEnglish,
                          //WebGoodNameKorea = p.WebGoodNameKorea,
                          //WebGoodNameVietNam = p.WebGoodNameVietNam,
                          //WebPriceEnglish = p.WebPriceEnglish,
                          //WebPriceKorea = p.WebPriceKorea,
                          //WebPriceVietNam = p.WebPriceVietNam,
                          TaxVAT = p.SalePrice * t.Percent / 100,
                          isPromotion = p.isPromotion,
                          TaxRateId = p.TaxRateId,
                          Net = p.Net,
                          OpeningStockQuantityNB = p.OpeningStockQuantityNB,
                          GoodsQuotaId = p.GoodsQuotaId
                      };

        // Filter category type
        if (!string.IsNullOrEmpty(param.CategoryTypesSearch))
        {
            var categoryArr = new string[4];
            categoryArr = param.CategoryTypesSearch.Split(',');
            if (categoryArr.Length > 0)
            {
                for (int i = 0; i < categoryArr.Length; i++)
                {
                    switch (categoryArr[i].Substring(0, 1))
                    {
                        case "1":
                            results = results.Where(x => x.MenuType == categoryArr[i].Substring(1, categoryArr[i].Length - 1)); break;
                        case "2":
                            results = results.Where(x => x.GoodsType == categoryArr[i].Substring(1, categoryArr[i].Length - 1)); break;
                        case "3":
                            results = results.Where(x => x.Position == categoryArr[i].Substring(1, categoryArr[i].Length - 1)); break;
                        case "4":
                            results = results.Where(x => x.PriceList == categoryArr[i].Substring(1, categoryArr[i].Length - 1)); break;
                    }
                }
            }
        }
        return results;
    }

    public async Task<IEnumerable<SelectListModel>> GetAllGoodShowWeb()
    {
        var priceList = await _context.Categories.FirstOrDefaultAsync(x => x.IsShowWeb
                            && x.Type == (int)CategoryEnum.PriceList);
        string priceListCode = "BGC";
        if (priceList != null)
        {
            priceListCode = priceList.Code;
        }
        return await _context.Goods.Where(x => !x.IsDeleted && x.PriceList == priceListCode)
            .Select(x => new SelectListModel()
            {
                Id = x.Id,
                Code = !string.IsNullOrEmpty(x.Detail2) ? x.Detail2 : x.Detail1,
                Name = !string.IsNullOrEmpty(x.DetailName2) ? x.DetailName2 : x.DetailName1
            }).ToListAsync();
    }

    private async Task<List<GoodsExportlModel>> GetGoodConvert(List<GoodsExportlModel> goods)
    {

        var goodConverts = await _context.ConvertProducts.ToListAsync();
        foreach (var good in goods)
        {
            var goodConvert = goodConverts
                                     .FirstOrDefault(x => x.OppositeAccount == good.Account
                                                                 && x.OppositeDetail1 == good.Detail1
                                                                 && (string.IsNullOrEmpty(x.OppositeDetail2) || x.OppositeDetail2 == good.Detail2)
                                                                 && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == good.Warehouse));
            if (goodConvert is null)
            {
                continue;
            }

            var goodCheck = goods.FirstOrDefault(x => x.Account == goodConvert.Account
                                                                && x.Detail1 == goodConvert.Detail1
                                                                 && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == goodConvert.Detail2)
                                                                 && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == goodConvert.Warehouse));
            if (goodCheck is null)
            {
                goodCheck = GetAll_Common(new SearchViewModel()).FirstOrDefault(x => x.Account == goodConvert.Account
                                                                && x.Detail1 == goodConvert.Detail1
                                                                 && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == goodConvert.Detail2)
                                                               && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == goodConvert.Warehouse));

                if (goodCheck is null)
                    continue;
            }

            var quantityTotal = (goodCheck.Quantity ?? 0) * goodConvert.ConvertQuantity / goodConvert.Quantity + (good.Quantity ?? 0);

            // số lượng hộp 
            goodCheck.Quantity = Math.Floor(quantityTotal * goodConvert.Quantity / goodConvert.ConvertQuantity);
            // số lượng viên
            good.Quantity = quantityTotal - (goodCheck.Quantity * goodConvert.ConvertQuantity / goodConvert.Quantity);
        }
        return goods;
    }

    public async Task<byte[]> ExportExcelSCT(int year)
    {
        var listAccount = await _context.GetChartOfAccount(year)
            .Where(x => (x.Classification == 2 || x.Classification == 3)
                    && !string.IsNullOrEmpty(x.ParentRef)
                    && x.Type > 4
                    && !x.HasDetails)
            .Select(x => new
            {
                Code = x.Code,
                Name = x.Name,
                OpeningStockQuantityNb = x.OpeningStockQuantityNB
            }).ToListAsync();

        var goodsList = await _context.Goods.ToListAsync();
        var ledgersList = await _context.Ledgers.Where(l => l.IsInternal == 3).ToListAsync();

        var importresult = goodsList
            .Select(goods =>
            {
                // Tìm tất cả các Ledgers thỏa mãn điều kiện so sánh
                var matchedLedgers = ledgersList
                    .Where(ledger => ledger.DebitCode == goods.Account
                                  && ledger.DebitWarehouse == goods.WarehouseName
                                  && ledger.DebitDetailCodeFirst == goods.Detail1
                                  && ledger.DebitDetailCodeSecond == goods.Detail2)
                    .ToList();

                // Xác định Name
                var name = !string.IsNullOrEmpty(goods.Detail1) ? goods.Detail1 : goods.Detail2;

                // Xác định ParentRef
                var parentRef = (!string.IsNullOrEmpty(goods.Detail1) && goods.Detail1 == goods.Detail1)
                                ? goods.Account
                                : $"{goods.Account}:{goods.Detail1}";

                // Tính tổng Amount từ Ledgers
                var totalQuantity = matchedLedgers.Sum(l => l.Quantity);

                return new
                {
                    TotalQuantity = totalQuantity
                };
            }).ToList();

        var exportresult = goodsList
            .Select(goods =>
            {
                // Tìm tất cả các Ledgers thỏa mãn điều kiện so sánh
                var matchedLedgers = ledgersList
                    .Where(ledger => ledger.CreditCode == goods.Account
                                  && ledger.CreditWarehouse == goods.WarehouseName
                                  && ledger.CreditDetailCodeFirst == goods.Detail1
                                  && ledger.CreditDetailCodeSecond == goods.Detail2)
                    .ToList();

                // Xác định Name
                var name = !string.IsNullOrEmpty(goods.Detail1) ? goods.Detail1 : goods.Detail2;

                // Xác định ParentRef
                var parentRef = (!string.IsNullOrEmpty(goods.Detail1) && goods.Detail1 == goods.Detail1)
                                ? goods.Account
                                : $"{goods.Account}:{goods.Detail1}";

                // Tính tổng Amount từ Ledgers
                var totalQuantity = matchedLedgers.Sum(l => l.Quantity);

                return new
                {
                    TotalQuantity = totalQuantity
                };
            }).ToList();

        //tạo file excel
        string fileMapServer = $"SoChiTietDSHH_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        string folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\EXCEL");
        string pathSave = Path.Combine(folder, fileMapServer);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\SoChiTietDSHH.xlsx");

        using (FileStream templateStream = System.IO.File.OpenRead(fullPath))
        using (ExcelPackage package = new ExcelPackage(templateStream))
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
            int row = 4;
            int i = 0;

            foreach (var goods in listAccount)
            {
                i++;
                sheet.Cells[row, 1].Value = i.ToString();
                sheet.Cells[row, 2].Value = goods.Code;
                sheet.Cells[row, 3].Value = goods.Name;
                sheet.Cells[row, 4].Value = goods.OpeningStockQuantityNb;

                var importtotal = 0;
                var exporttotal = 0;

                if (importresult.Any()) // Kiểm tra có dữ liệu trong result hay không
                {
                    importtotal = (int)importresult[i - 1].TotalQuantity; // result có số lượng giống với listAccount
                    sheet.Cells[row, 5].Value = importtotal;
                }
                if (exportresult.Any())
                {
                    exporttotal = (int)exportresult[i - 1].TotalQuantity;
                    sheet.Cells[row, 6].Value = exporttotal;
                }
                var result = (goods.OpeningStockQuantityNb + importtotal - exporttotal);
                sheet.Cells[row, 7].Value = result;

                row++;
            }

            var range = sheet.Cells[4, 1, row - 1, 7];
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

            sheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
    public async Task<byte[]> ExportExcelCVP(int year) //convertproduct
    {
        var oppositeDetails = _context.ConvertProducts
            .Select(x => new
            {
                x.OppositeDetail1,
                x.OppositeDetailName1,
                x.OppositeDetail2,
                x.OppositeDetailName2
            })
            .ToList();
        var oppositeDetailSet = new HashSet<string>(
            oppositeDetails.SelectMany(x => new[] { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
            .Where(x => !string.IsNullOrEmpty(x))
        );
        var listAccount = await _context.GetChartOfAccount(year)
            .Where(x => (x.Classification == 2 || x.Classification == 3)
                && !string.IsNullOrEmpty(x.ParentRef)
                && x.Type > 4
                && !x.HasDetails)
            .Select(x => new
            {
                Code = x.Code,
                Name = x.Name,
                OpeningStockQuantityNb = x.OpeningStockQuantityNB
            })
            .Where(x => !oppositeDetailSet.Contains(x.Code) && !oppositeDetailSet.Contains(x.Name))
            .ToListAsync();
        var originalListAccount = await _context.GetChartOfAccount(year)
            .Where(x => (x.Classification == 2 || x.Classification == 3)
                && !string.IsNullOrEmpty(x.ParentRef)
                && x.Type > 4
                && !x.HasDetails)
            .Select(x => new
            {
                Code = x.Code,
                Name = x.Name,
                OpeningStockQuantityNb = x.OpeningStockQuantityNB
            })
            .ToListAsync();
        var goodsList = await _context.Goods.ToListAsync();
        var ledgersList = await _context.Ledgers.Where(l => l.IsInternal == 3).ToListAsync();

        var importresult = goodsList
            .Select(goods =>
            {
                // Tìm tất cả các Ledgers thỏa mãn điều kiện so sánh
                var matchedLedgers = ledgersList
                    .Where(ledger => ledger.DebitCode == goods.Account
                                  && ledger.DebitWarehouse == goods.WarehouseName
                                  && ledger.DebitDetailCodeFirst == goods.Detail1
                                  && ledger.DebitDetailCodeSecond == goods.Detail2)
                    .ToList();

                // Xác định Name
                var name = !string.IsNullOrEmpty(goods.Detail1) ? goods.Detail1 : goods.Detail2;

                // Xác định ParentRef
                var parentRef = (!string.IsNullOrEmpty(goods.Detail1) && goods.Detail1 == goods.Detail1)
                                ? goods.Account
                                : $"{goods.Account}:{goods.Detail1}";

                // Tính tổng Amount từ Ledgers
                var totalQuantity = matchedLedgers.Sum(l => l.Quantity);
                var totalAmount = matchedLedgers.Sum(l => l.Amount); // ➕ Lấy thêm tổng tiền

                return new
                {
                    TotalAmount = totalAmount,
                    TotalQuantity = totalQuantity
                };
            }).ToList();

        var exportresult = goodsList
            .Select(goods =>
            {
                // Tìm tất cả các Ledgers thỏa mãn điều kiện so sánh
                var matchedLedgers = ledgersList
                    .Where(ledger => ledger.CreditCode == goods.Account
                                  && ledger.CreditWarehouse == goods.WarehouseName
                                  && ledger.CreditDetailCodeFirst == goods.Detail1
                                  && ledger.CreditDetailCodeSecond == goods.Detail2)
                    .ToList();

                // Xác định Name
                var name = !string.IsNullOrEmpty(goods.Detail1) ? goods.Detail1 : goods.Detail2;

                // Xác định ParentRef
                var parentRef = (!string.IsNullOrEmpty(goods.Detail1) && goods.Detail1 == goods.Detail1)
                                ? goods.Account
                                : $"{goods.Account}:{goods.Detail1}";

                // Tính tổng Amount từ Ledgers
                var totalQuantity = matchedLedgers.Sum(l => l.Quantity);

                return new
                {

                    TotalQuantity = totalQuantity
                };
            }).ToList();

        //tạo file excel
        string fileMapServer = $"ChiTietDSHH_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        string folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\EXCEL");
        string pathSave = Path.Combine(folder, fileMapServer);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\ChiTietDSHH.xlsx");

        using (FileStream templateStream = System.IO.File.OpenRead(fullPath))
        using (ExcelPackage package = new ExcelPackage(templateStream))
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
            // Tạo HashSet để lưu trữ các mã hàng đã được thêm vào Excel
            HashSet<string> addedProducts = new HashSet<string>();

            int row = 4;
            int i = 0;
            foreach (var goods in listAccount)
            {
                i++;
                sheet.Cells[row, 1].Value = i.ToString();
                sheet.Cells[row, 2].Value = goods.Code;
                sheet.Cells[row, 3].Value = goods.Name;
                sheet.Cells[row, 4].Value = goods.OpeningStockQuantityNb;

                var importtotal = 0;
                var exporttotal = 0;

                if (importresult.Any()) // Kiểm tra có dữ liệu trong result hay không
                {
                    importtotal = (int)importresult[i - 1].TotalQuantity; // result có số lượng giống với listAccount
                    sheet.Cells[row, 5].Value = importtotal;

                    var importAmount = (int)importresult[i - 1].TotalAmount;
                    var costPrice = importtotal > 0 ? (double)importAmount / importtotal : 0;
                    sheet.Cells[row, 8].Value = costPrice;
                }
                if (exportresult.Any())
                {
                    exporttotal = (int)exportresult[i - 1].TotalQuantity;
                    sheet.Cells[row, 6].Value = exporttotal;
                }
                var result = (goods.OpeningStockQuantityNb + importtotal - exporttotal);
                sheet.Cells[row, 7].Value = result;


                var matchedProduct = _context.ConvertProducts
                .FirstOrDefault(cp => cp.Detail1 == goods.Code && cp.DetailName1 == goods.Name);
                if (matchedProduct != null)
                {
                    string key = $"{matchedProduct.OppositeDetail1}-{matchedProduct.OppositeDetailName1}"; // Tạo khóa duy nhất

                    if (!addedProducts.Contains(key)) // Chỉ thêm nếu chưa tồn tại
                    {
                        sheet.Cells[row, 9].Value = matchedProduct.OppositeDetail1; // Mã hàng con
                        sheet.Cells[row, 10].Value = matchedProduct.OppositeDetailName1; // Tên hàng con
                        var matchedAccount = originalListAccount
                                            .FirstOrDefault(x => x.Code == matchedProduct.OppositeDetail1 && x.Name == matchedProduct.OppositeDetailName1);

                        if (matchedAccount != null)
                        {
                            sheet.Cells[row, 11].Value = matchedAccount.OpeningStockQuantityNb; // Tồn đầu kỳ
                        }

                        if (importresult.Any()) // Kiểm tra có dữ liệu trong result hay không
                        {
                            importtotal = (int)importresult[i - 1].TotalQuantity; // result có số lượng giống với listAccount
                            sheet.Cells[row, 5].Value = importtotal;

                            var importAmount = (int)importresult[i - 1].TotalAmount;
                            var costPrice = importtotal > 0 ? (double)importAmount / importtotal : 0;
                            sheet.Cells[row, 8].Value = costPrice;
                        }
                        if (exportresult.Any())
                        {
                            exporttotal = (int)exportresult[i - 1].TotalQuantity;
                            sheet.Cells[row, 6].Value = exporttotal;
                        }

                        sheet.Cells[row, 12].Value = importtotal; // Nhập
                        sheet.Cells[row, 13].Value = exporttotal; // Xuất
                        sheet.Cells[row, 14].Value = (matchedAccount?.OpeningStockQuantityNb ?? 0) + importtotal - exporttotal; // Tồn cuối kỳ

                        addedProducts.Add(key); // Đánh dấu là đã thêm
                    }
                }
                row++;
            }
            sheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
    //Phong's new commit
    //public async Task<List<InventoryProductStockViewModel>> GetConvertProductStockData(int year)
    //{
    //    var oppositeDetails = _context.ConvertProducts
    //        .Select(x => new { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
    //        .ToList();

    //    var oppositeDetailSet = new HashSet<string>(
    //        oppositeDetails.SelectMany(x => new[] { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
    //        .Where(x => !string.IsNullOrEmpty(x))
    //    );

    //    var listAccount = await _context.GetChartOfAccount(year)
    //        .Where(x => (x.Classification == 2 || x.Classification == 3)
    //            && !string.IsNullOrEmpty(x.ParentRef)
    //            && x.Type > 4
    //            && !x.HasDetails)
    //        .Select(x => new { x.ParentRef, x.Code, x.Name, x.OpeningStockQuantityNB })
    //        .Where(x => !oppositeDetailSet.Contains(x.Code) && !oppositeDetailSet.Contains(x.Name))
    //        .ToListAsync();

    //    var originalListAccount = await _context.GetChartOfAccount(year)
    //        .Where(x => (x.Classification == 2 || x.Classification == 3)
    //            && !string.IsNullOrEmpty(x.ParentRef)
    //            && x.Type > 4
    //            && !x.HasDetails)
    //        .Select(x => new {x.ParentRef, x.Code, x.Name, x.OpeningStockQuantityNB })
    //        .ToListAsync();

    //    var goodsList = await _context.Goods.ToListAsync();
    //    var ledgersList = _context.GetLedger(year, 2);
    //    var resultList = new List<InventoryProductStockViewModel>();
    //    var addedProducts = new HashSet<string>();

    //    for (int i = 0; i < listAccount.Count; i++)
    //    {
    //        var item = listAccount[i];
    //        var goods = new Goods();
    //        bool found = false;
    //        foreach (var good in goodsList)
    //        {
    //            var name = (!string.IsNullOrEmpty(good.Detail2))
    //                        ? good.DetailName2
    //                        : good.DetailName1;
    //            var code = (!string.IsNullOrEmpty(good.Detail2))
    //                            ? good.Detail2
    //                            : good.Detail1;
    //            if (name == item.Name && code== item.Code)
    //            {
    //                goods = good;
    //                found = true;
    //                break;
    //            }
    //        }
    //        if (!found)
    //        {
    //            continue;
    //        }

    //        var imports = ledgersList
    //            .Where(ledger =>
    //            ledger.DebitCode == goods.Account &&
    //            (string.IsNullOrEmpty(ledger.DebitWarehouse) || ledger.DebitWarehouse == goods.WarehouseName) &&
    //            ledger.DebitDetailCodeFirst == goods.Detail1 &&
    //            ledger.DebitDetailCodeSecond == goods.Detail2
    //        ).ToList();

    //        var exports = ledgersList.Where(ledger =>
    //            ledger.CreditCode == goods.Account &&
    //            (string.IsNullOrEmpty(ledger.CreditWarehouse) || ledger.CreditWarehouse == goods.WarehouseName) &&
    //            ledger.CreditDetailCodeFirst == goods.Detail1 &&
    //            ledger.CreditDetailCodeSecond == goods.Detail2
    //        ).ToList();

    //        var importtotal = imports?.Sum(p => p.Quantity) ?? 0;
    //        var exporttotal = exports?.Sum(p => p.Quantity) ?? 0;
    //        double costPrice = 0;
    //        if(imports.Count() > 0)
    //        {
    //            foreach (var imp in imports)
    //            {
    //                if (imp.Amount >= 0 && imp.Quantity > 0)
    //                {
    //                    costPrice += (imp.Amount / imp.Quantity) / imports.Count();
    //                }
    //            }
    //        }

    //        //Tinh gia von trung binh / thung -> lon
    //        var matchedProduct = _context.ConvertProducts
    //            .FirstOrDefault(cp => 
    //            cp.Account == goods.Account && 
    //            cp.Detail1 == goods.Detail1 && 
    //            cp.Detail2 == goods.Detail2);
    //        var costAvgConvertQuantity = matchedProduct != null ? (costPrice / matchedProduct.ConvertQuantity) : 0;

    //        var ending = (item.OpeningStockQuantityNB ?? 0) + importtotal - exporttotal;

    //        var model = new InventoryProductStockViewModel
    //        {
    //            Index = i + 1,
    //            Code = item.Code, //goods
    //            Name = item.Name, //goods
    //            OpeningStock = (int)(item.OpeningStockQuantityNB ?? 0), //goods
    //            ImportQuantity = (int)importtotal,
    //            ExportQuantity = (int)exporttotal,
    //            EndingStock = (int)ending,
    //            CostPrice = (int)costPrice,
    //            EndingValue = costAvgConvertQuantity,
    //        };

    //        if (matchedProduct != null)
    //        {
    //            var name = (!string.IsNullOrEmpty(matchedProduct.OppositeDetail2))
    //                        ? matchedProduct.OppositeDetailName2
    //                        : matchedProduct.OppositeDetailName1;
    //            var code = (!string.IsNullOrEmpty(matchedProduct.OppositeDetail2))
    //                            ? matchedProduct.OppositeDetail2
    //                            : matchedProduct.OppositeDetail1;
    //            model.OppositeCode = code;
    //            model.OppositeName = name;

    //            var impOpposite = ledgersList
    //                .Where(ledger => ledger.DebitCode == matchedProduct.Account
    //                              && (string.IsNullOrEmpty(ledger.DebitWarehouse) || ledger.DebitWarehouse == matchedProduct.WarehouseName)
    //                              && ledger.DebitDetailCodeFirst == matchedProduct.OppositeDetail1
    //                              && ledger.DebitDetailCodeSecond == matchedProduct.OppositeDetail2)
    //                .ToList();
    //            var expOpposite = ledgersList
    //                .Where(ledger => ledger.CreditCode == matchedProduct.Account
    //                              && (string.IsNullOrEmpty(ledger.CreditWarehouse) || ledger.CreditWarehouse == matchedProduct.WarehouseName)
    //                              && ledger.CreditDetailCodeFirst == matchedProduct.OppositeDetail1
    //                              && ledger.CreditDetailCodeSecond == matchedProduct.OppositeDetail2)
    //                .ToList();

    //            var matchedAccount = originalListAccount
    //                .FirstOrDefault(x => (x.Code == matchedProduct.OppositeDetail1 && x.Name == matchedProduct.OppositeDetailName1) ||
    //                (x.Code == matchedProduct.OppositeDetail2 && x.Name == matchedProduct.OppositeDetailName2));
    //            double oppCost = 0;
    //            if(impOpposite.Count() > 0)
    //            {
    //                foreach(var imp in impOpposite)
    //                {
    //                    if (imp.Amount >= 0 && imp.Quantity > 0)
    //                    {
    //                        oppCost += (imp.Amount / imp.Quantity) / impOpposite.Count();
    //                    }
    //                }
    //            }
    //            var avg_value = (costAvgConvertQuantity + oppCost) / 2;
    //            var openingOpp = (int)(matchedAccount?.OpeningStockQuantityNB ?? 0);
    //            var opp_ending_stock = (int)(openingOpp + impOpposite.Sum(p => p.Quantity) - expOpposite.Sum(p => p.Quantity));
    //            model.OppositeOpeningStock = openingOpp;
    //            model.OppositeImportQuantity = (int)impOpposite.Sum(p=>p.Quantity);
    //            model.OppositeExportQuantity = (int)expOpposite.Sum(p=>p.Quantity);
    //            model.OppositeEndingStock = opp_ending_stock;
    //            model.OppositeEndingCost = (int)oppCost;
    //            model.EndingAvgValue = (int)avg_value;
    //            model.EndingTotalValue = (int) avg_value * (opp_ending_stock + matchedProduct.ConvertQuantity * ending);
    //        }
    //        resultList.Add(model);
    //    }
    //    return resultList;
    //}
    public async Task<PagingResult<InventoryProductStockViewModel>> GetConvertProductStockData(int year, int page, int pageSize)
    {
        var result = new PagingResult<InventoryProductStockViewModel>
        {
            CurrentPage = page,
            PageSize = pageSize
        };

        var oppositeDetails = _context.ConvertProducts
            .Select(x => new { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
            .ToList();

        var oppositeSet = new HashSet<string>(
            oppositeDetails.SelectMany(x => new[] { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
                           .Where(x => !string.IsNullOrEmpty(x))
        );

        var fullAccounts = await _context.GetChartOfAccount(year)
            .Where(x => (x.Classification == 2 || x.Classification == 3)
                        && !string.IsNullOrEmpty(x.ParentRef)
                        && x.Type > 4
                        && !x.HasDetails)
            .Select(x => new { x.ParentRef, x.Code, x.Name, x.OpeningStockQuantityNB, x.OpeningDebitNB, x.StockUnit }).ToListAsync();

        var filteredAccounts = fullAccounts
            .Where(x => !oppositeSet.Contains(x.Code) && !oppositeSet.Contains(x.Name))
            .ToList();

        result.TotalItems = filteredAccounts.Count();

        var pagedAccounts = filteredAccounts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var codeList = fullAccounts.Select(x => x.Code).ToHashSet();

        var goodsList = await _context.Goods.ToListAsync();
        var ledgers = await _context.Ledgers
                            .Where(p => p.IsInternal == 3 &&
                                        (codeList.Contains(p.DebitDetailCodeFirst) || codeList.Contains(p.DebitDetailCodeSecond) ||
                                         codeList.Contains(p.CreditDetailCodeFirst) || codeList.Contains(p.CreditDetailCodeSecond)))
                            .ToListAsync();

        for (int i = 0; i < pagedAccounts.Count(); i++)
        {
            var acc = pagedAccounts[i];
            Goods goods = null;

            foreach (var g in goodsList)
            {
                var code = !string.IsNullOrEmpty(g.Detail2) ? g.Detail2 : g.Detail1;
                var name = !string.IsNullOrEmpty(g.Detail2) ? g.DetailName2 : g.DetailName1;
                if (acc.Code == code && acc.Name == name)
                {
                    goods = g;
                    break;
                }
            }

            if (goods == null) continue;

            var imports = ledgers.Where(l =>
                l.DebitCode == goods.Account &&
                (string.IsNullOrEmpty(l.DebitWarehouse) || l.DebitWarehouse == goods.WarehouseName) &&
                l.DebitDetailCodeFirst == goods.Detail1 &&
                l.DebitDetailCodeSecond == goods.Detail2).ToList();

            var exports = ledgers.Where(l =>
                l.CreditCode == goods.Account &&
                (string.IsNullOrEmpty(l.CreditWarehouse) || l.CreditWarehouse == goods.WarehouseName) &&
                l.CreditDetailCodeFirst == goods.Detail1 &&
                l.CreditDetailCodeSecond == goods.Detail2).ToList();

            double costPrice = 0;
            var validImports = imports.Where(l => l.Amount >= 0 && l.Quantity > 0).ToList();
            if (validImports.Any())
                costPrice = (validImports.Sum(l => l.Amount) + (acc.OpeningDebitNB ?? 0)) / (validImports.Sum(p => p.Quantity) + (acc.OpeningStockQuantityNB ?? 0));

            var endingStock = (acc.OpeningStockQuantityNB ?? 0) + imports.Sum(x => x.Quantity) - exports.Sum(x => x.Quantity);

            var matchedProduct = _context.ConvertProducts.FirstOrDefault(cp =>
                cp.Account == goods.Account &&
                cp.Detail1 == goods.Detail1 &&
                cp.Detail2 == goods.Detail2);

            double convertAvgCost = (matchedProduct?.ConvertQuantity > 0) ? (costPrice / matchedProduct.ConvertQuantity) : 0;

            var model = new InventoryProductStockViewModel
            {
                Index = ((page - 1) * pageSize) + i + 1,
                Code = acc.Code,
                Name = acc.Name,
                OpeningStock = (int)(acc.OpeningStockQuantityNB ?? 0),
                ImportQuantity = (int)imports.Sum(x => x.Quantity),
                ExportQuantity = (int)exports.Sum(x => x.Quantity),
                EndingStock = (int)endingStock,
                CostPrice = (int)costPrice,
                EndingValue = convertAvgCost
            };

            if (matchedProduct != null)
            {
                var oppCode = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetail2 : matchedProduct.OppositeDetail1;
                var oppName = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetailName2 : matchedProduct.OppositeDetailName1;

                model.OppositeCode = oppCode;
                model.OppositeName = oppName;

                var oppImports = ledgers.Where(l =>
                    l.DebitCode == matchedProduct.Account &&
                    (string.IsNullOrEmpty(l.DebitWarehouse) || l.DebitWarehouse == matchedProduct.WarehouseName) &&
                    l.DebitDetailCodeFirst == matchedProduct.OppositeDetail1 &&
                    l.DebitDetailCodeSecond == matchedProduct.OppositeDetail2).ToList();

                var oppExports = ledgers.Where(l =>
                    l.CreditCode == matchedProduct.Account &&
                    (string.IsNullOrEmpty(l.CreditWarehouse) || l.CreditWarehouse == matchedProduct.WarehouseName) &&
                    l.CreditDetailCodeFirst == matchedProduct.OppositeDetail1 &&
                    l.CreditDetailCodeSecond == matchedProduct.OppositeDetail2).ToList();

                var matchedAcc = fullAccounts.FirstOrDefault(x =>
                    (x.Code == matchedProduct.OppositeDetail1 && x.Name == matchedProduct.OppositeDetailName1) ||
                    (x.Code == matchedProduct.OppositeDetail2 && x.Name == matchedProduct.OppositeDetailName2));

                double oppCost = 0;
                var validOppImports = oppImports.Where(l => l.Amount >= 0 && l.Quantity > 0).ToList();
                if (validOppImports.Any())
                    oppCost = (validOppImports.Sum(l => l.Amount) + (matchedAcc.OpeningDebitNB ?? 0)) / (validOppImports.Sum(l => l.Quantity) + (matchedAcc.OpeningStockQuantityNB ?? 0));

                double avgValue = oppCost == 0 ? convertAvgCost : (convertAvgCost + oppCost) / 2;
                int openingOpp = (int)(matchedAcc?.OpeningStockQuantityNB ?? 0);
                int oppEnding = (int)(openingOpp + oppImports.Sum(x => x.Quantity) - oppExports.Sum(x => x.Quantity));
                var endingTotal = oppEnding + (matchedProduct.ConvertQuantity * endingStock);
                model.OppositeOpeningStock = openingOpp;
                model.OppositeImportQuantity = (int)oppImports.Sum(x => x.Quantity);
                model.OppositeExportQuantity = (int)oppExports.Sum(x => x.Quantity);
                model.OppositeEndingStock = oppEnding;
                model.OppositeEndingCost = (int)oppCost;
                model.EndingAvgValue = (int)avgValue;
                model.EndingTotalValue = (int)(avgValue * (oppEnding + (matchedProduct.ConvertQuantity * endingStock)));
                var unitOrigin = acc.StockUnit;
                var unitConvert = matchedAcc != null ? matchedAcc.StockUnit : "";
                int endingOrigin = (int)endingTotal / matchedProduct.ConvertQuantity;
                int endingConvert = (int)endingTotal % matchedProduct.ConvertQuantity;
                model.EndingString = $"{endingOrigin} {unitOrigin} & {endingConvert} {unitConvert}";
            }
            result.Data.Add(model);
        }
        return result;
    }
    //public async Task<List<InventoryProductStockViewModel>> GetConvertProductStockData(int year)
    //{
    //    var result = new List<InventoryProductStockViewModel>();

    //    var oppositeDetails = _context.ConvertProducts
    //        .Select(x => new { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
    //        .ToList();

    //    var oppositeSet = new HashSet<string>(
    //        oppositeDetails.SelectMany(x => new[] { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
    //                       .Where(x => !string.IsNullOrEmpty(x))
    //    );

    //    var fullAccounts = await _context.GetChartOfAccount(year)
    //        .Where(x => (x.Classification == 2 || x.Classification == 3)
    //                    && !string.IsNullOrEmpty(x.ParentRef)
    //                    && x.Type > 4
    //                    && !x.HasDetails)
    //        .Select(x => new {x.WarehouseName, x.ParentRef, x.Code, x.Name, x.OpeningStockQuantityNB, x.OpeningDebitNB, x.StockUnit }).ToListAsync();

    //    var filteredAccounts = fullAccounts
    //        .Where(x => !oppositeSet.Contains(x.Code) && !oppositeSet.Contains(x.Name))
    //        .ToList();

    //    var codeList = fullAccounts.Select(x => x.Code).ToHashSet();

    //    var goodsList = await _context.Goods.ToListAsync();
    //    var ledgers = await _context.Ledgers
    //                        .Where(p => p.IsInternal == 3 &&
    //                                    (codeList.Contains(p.DebitDetailCodeFirst) || codeList.Contains(p.DebitDetailCodeSecond) ||
    //                                     codeList.Contains(p.CreditDetailCodeFirst) || codeList.Contains(p.CreditDetailCodeSecond)))
    //                        .ToListAsync();

    //    for (int i = 0; i < filteredAccounts.Count(); i++)
    //    {
    //        var acc = filteredAccounts[i];
    //        var parts = acc.ParentRef.Split(':');
    //        var acc_account = parts[0];
    //        string acc_detail1 = parts.Length > 1 ? parts[1] : acc.Code;
    //        string acc_detail2 = parts.Length > 1 ? acc.Code : null;

    //        var imports = ledgers.Where(l =>
    //            l.DebitCode == acc_account &&
    //            (string.IsNullOrEmpty(l.DebitWarehouseName) || l.DebitWarehouse == acc.WarehouseName) &&
    //            l.DebitDetailCodeFirst == acc_detail1 &&
    //            (string.IsNullOrEmpty(acc_detail2) || l.DebitDetailCodeSecond == acc_detail2)).ToList();

    //        var exports = ledgers.Where(l =>
    //            l.CreditCode == acc_account &&
    //            (string.IsNullOrEmpty(l.CreditWarehouseName) || l.DebitWarehouse == acc.WarehouseName) &&
    //            l.CreditDetailCodeFirst == acc_detail1 &&
    //            (string.IsNullOrEmpty(acc_detail2) || l.CreditDetailCodeSecond == acc_detail2)).ToList();

    //        double costPrice = 0;
    //        var validImports = imports.Where(l => l.Amount >= 0 && l.Quantity > 0).ToList();
    //        if (validImports.Any())
    //            costPrice = (validImports.Sum(l => l.Amount) + (acc.OpeningDebitNB ?? 0)) / (validImports.Sum(p => p.Quantity) + (acc.OpeningStockQuantityNB ?? 0));

    //        var endingStock = (acc.OpeningStockQuantityNB ?? 0) + imports.Sum(x => x.Quantity) - exports.Sum(x => x.Quantity);

    //        var matchedProduct = _context.ConvertProducts.FirstOrDefault(cp =>
    //            cp.Account == acc_account &&
    //            cp.Detail1 == acc_detail1 &&
    //            (string.IsNullOrEmpty(acc_detail2) || cp.Detail2 == acc_detail2));

    //        double convertAvgCost = (matchedProduct?.ConvertQuantity > 0) ? (costPrice / matchedProduct.ConvertQuantity) : 0;

    //        var model = new InventoryProductStockViewModel
    //        {
    //            Index = i + 1,
    //            Code = acc.Code,
    //            Name = acc.Name,
    //            OpeningStock = (int)(acc.OpeningStockQuantityNB ?? 0),
    //            ImportQuantity = (int)imports.Sum(x => x.Quantity),
    //            ExportQuantity = (int)exports.Sum(x => x.Quantity),
    //            EndingStock = (int)endingStock,
    //            CostPrice = (int)costPrice,
    //            EndingValue = convertAvgCost
    //        };

    //        if (matchedProduct != null)
    //        {
    //            var oppCode = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetail2 : matchedProduct.OppositeDetail1;
    //            var oppName = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetailName2 : matchedProduct.OppositeDetailName1;

    //            model.OppositeCode = oppCode;
    //            model.OppositeName = oppName;

    //            var oppImports = ledgers.Where(l =>
    //                l.DebitCode == matchedProduct.Account &&
    //                (string.IsNullOrEmpty(l.DebitWarehouse) || l.DebitWarehouse == matchedProduct.WarehouseName) &&
    //                l.DebitDetailCodeFirst == matchedProduct.OppositeDetail1 &&
    //                l.DebitDetailCodeSecond == matchedProduct.OppositeDetail2).ToList();

    //            var oppExports = ledgers.Where(l =>
    //                l.CreditCode == matchedProduct.Account &&
    //                (string.IsNullOrEmpty(l.CreditWarehouse) || l.CreditWarehouse == matchedProduct.WarehouseName) &&
    //                l.CreditDetailCodeFirst == matchedProduct.OppositeDetail1 &&
    //                l.CreditDetailCodeSecond == matchedProduct.OppositeDetail2).ToList();

    //            var matchedAcc = fullAccounts.FirstOrDefault(x =>
    //                (x.Code == matchedProduct.OppositeDetail1 && x.Name == matchedProduct.OppositeDetailName1) ||
    //                (x.Code == matchedProduct.OppositeDetail2 && x.Name == matchedProduct.OppositeDetailName2));

    //            double oppCost = 0;
    //            var validOppImports = oppImports.Where(l => l.Amount >= 0 && l.Quantity > 0).ToList();
    //            if (validOppImports.Any())
    //                oppCost = (validOppImports.Sum(l => l.Amount) + (matchedAcc.OpeningDebitNB ?? 0)) / (validOppImports.Sum(l => l.Quantity) + (matchedAcc.OpeningStockQuantityNB ?? 0));

    //            double avgValue = oppCost == 0 ? convertAvgCost : (convertAvgCost + oppCost) / 2;
    //            int openingOpp = (int)(matchedAcc?.OpeningStockQuantityNB ?? 0);
    //            int oppEnding = (int)(openingOpp + oppImports.Sum(x => x.Quantity) - oppExports.Sum(x => x.Quantity));
    //            var endingTotal = oppEnding + (matchedProduct.ConvertQuantity * endingStock);
    //            model.OppositeOpeningStock = openingOpp;
    //            model.OppositeImportQuantity = (int)oppImports.Sum(x => x.Quantity);
    //            model.OppositeExportQuantity = (int)oppExports.Sum(x => x.Quantity);
    //            model.OppositeEndingStock = oppEnding;
    //            model.OppositeEndingCost = (int)oppCost;
    //            model.EndingAvgValue = (int)avgValue;
    //            model.EndingTotalValue = (int)(avgValue * (oppEnding + (matchedProduct.ConvertQuantity * endingStock)));
    //            var unitOrigin = acc.StockUnit;
    //            var unitConvert = matchedAcc != null ? matchedAcc.StockUnit : "";
    //            int endingOrigin = (int)endingTotal / matchedProduct.ConvertQuantity;
    //            int endingConvert = (int)endingTotal % matchedProduct.ConvertQuantity;
    //            model.EndingString = $"{endingOrigin} {unitOrigin} & {endingConvert} {unitConvert}";
    //        }
    //        result.Add(model);
    //    }
    //    return result;
    //}
    public async Task<List<InventoryProductStockViewModel>> GetConvertProductStockData(int year)
    {
        var result = new List<InventoryProductStockViewModel>();

        // 1. Load ConvertProducts và build lookup
        var convertProducts = await _context.ConvertProducts.ToListAsync();
        var convertProductMap = convertProducts
            .GroupBy(x => $"{x.Account}_{x.Detail1}_{x.Detail2}")
            .ToDictionary(g => g.Key, g => g.First());

        // 2. Build HashSet để lọc các đối tượng đối ứng
        var oppositeSet = new HashSet<string>(
            convertProducts
                .SelectMany(x => new[] { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
                .Where(x => !string.IsNullOrEmpty(x))
        );

        // 3. Lấy danh sách tài khoản gốc
        var chartAccounts = await _context.GetChartOfAccount(year)
            .Where(x => (x.Classification == 2 || x.Classification == 3)
                        && !string.IsNullOrEmpty(x.ParentRef)
                        && x.Type > 4
                        && !x.HasDetails)
            .ToListAsync();

        var codeSet = chartAccounts.Select(x => x.Code).ToHashSet();

        var filteredAccounts = chartAccounts
            .Where(x => !oppositeSet.Contains(x.Code) && !oppositeSet.Contains(x.Name))
            .ToList();

        // 4. Load ledger 1 lần duy nhất
        var ledgers = await _context.Ledgers
            .Where(p => p.IsInternal == 3 &&
                (codeSet.Contains(p.DebitDetailCodeFirst) || codeSet.Contains(p.DebitDetailCodeSecond) ||
                 codeSet.Contains(p.CreditDetailCodeFirst) || codeSet.Contains(p.CreditDetailCodeSecond)))
            .ToListAsync();

        // 5. Build Dictionary để truy cập ledger nhanh
        var debitLedgerMap = ledgers
            .GroupBy(l => $"{l.DebitCode}_{l.DebitDetailCodeFirst}_{l.DebitDetailCodeSecond}")
            .ToDictionary(g => g.Key, g => g.ToList());

        var creditLedgerMap = ledgers
            .GroupBy(l => $"{l.CreditCode}_{l.CreditDetailCodeFirst}_{l.CreditDetailCodeSecond}")
            .ToDictionary(g => g.Key, g => g.ToList());

        // 6. Bắt đầu xử lý từng account
        for (int i = 0; i < filteredAccounts.Count; i++)
        {
            var acc = filteredAccounts[i];
            var parts = acc.ParentRef.Split(':');
            var acc_account = parts[0];
            var acc_detail1 = parts.Length > 1 ? parts[1] : acc.Code;
            var acc_detail2 = parts.Length > 1 ? acc.Code : null;

            var key = $"{acc_account}_{acc_detail1}_{acc_detail2}";
            debitLedgerMap.TryGetValue(key, out var imports);
            creditLedgerMap.TryGetValue(key, out var exports);

            double openingQty = acc.OpeningStockQuantityNB ?? 0;
            double openingDebit = acc.OpeningDebitNB ?? 0;

            double totalImportQty = imports?.Sum(l => l.Quantity) ?? 0;
            double totalImportAmt = imports?.Where(l => l.Amount >= 0 && l.Quantity > 0).Sum(l => l.Amount) ?? 0;
            double validImportQty = imports?.Where(l => l.Amount >= 0 && l.Quantity > 0).Sum(l => l.Quantity) ?? 0;

            double totalExportQty = exports?.Sum(l => l.Quantity) ?? 0;

            double costPrice = validImportQty + openingQty > 0
                ? (totalImportAmt + openingDebit) / (validImportQty + openingQty)
                : 0;

            double endingStock = openingQty + totalImportQty - totalExportQty;

            // ConvertProduct tra từ Dictionary
            convertProductMap.TryGetValue(key, out var matchedProduct);
            double convertAvgCost = (matchedProduct?.ConvertQuantity > 0) ? (costPrice / matchedProduct.ConvertQuantity) : 0;

            var model = new InventoryProductStockViewModel
            {
                Index = i + 1,
                Code = acc.Code,
                Name = acc.Name,
                OpeningStock = (int)openingQty,
                ImportQuantity = (int)totalImportQty,
                ExportQuantity = (int)totalExportQty,
                EndingStock = (int)endingStock,
                CostPrice = (int)costPrice,
                EndingValue = convertAvgCost
            };

            if (matchedProduct != null)
            {
                var oppCode = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetail2 : matchedProduct.OppositeDetail1;
                var oppName = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetailName2 : matchedProduct.OppositeDetailName1;

                model.OppositeCode = oppCode;
                model.OppositeName = oppName;

                var oppKey = $"{matchedProduct.Account}_{matchedProduct.OppositeDetail1}_{matchedProduct.OppositeDetail2}";
                debitLedgerMap.TryGetValue(oppKey, out var oppImports);
                creditLedgerMap.TryGetValue(oppKey, out var oppExports);

                var matchedAcc = chartAccounts.FirstOrDefault(x =>
                    (x.Code == matchedProduct.OppositeDetail1 && x.Name == matchedProduct.OppositeDetailName1) ||
                    (x.Code == matchedProduct.OppositeDetail2 && x.Name == matchedProduct.OppositeDetailName2));

                double oppOpeningQty = matchedAcc?.OpeningStockQuantityNB ?? 0;
                double oppOpeningDebit = matchedAcc?.OpeningDebitNB ?? 0;

                double oppImportQty = oppImports?.Sum(x => x.Quantity) ?? 0;
                double oppImportAmt = oppImports?.Where(x => x.Amount >= 0 && x.Quantity > 0).Sum(x => x.Amount) ?? 0;
                double oppValidQty = oppImports?.Where(x => x.Amount >= 0 && x.Quantity > 0).Sum(x => x.Quantity) ?? 0;
                double oppExportQty = oppExports?.Sum(x => x.Quantity) ?? 0;

                double oppCost = (oppValidQty + oppOpeningQty > 0)
                    ? (oppImportAmt + oppOpeningDebit) / (oppValidQty + oppOpeningQty)
                    : 0;

                double avgValue = oppCost == 0 ? convertAvgCost : (convertAvgCost + oppCost) / 2;

                int oppEnding = (int)(oppOpeningQty + oppImportQty - oppExportQty);
                int endingTotal = oppEnding + (int)(matchedProduct.ConvertQuantity * endingStock);

                model.OppositeOpeningStock = (int)oppOpeningQty;
                model.OppositeImportQuantity = (int)oppImportQty;
                model.OppositeExportQuantity = (int)oppExportQty;
                model.OppositeEndingStock = oppEnding;
                model.OppositeEndingCost = (int)oppCost;
                model.EndingAvgValue = (int)avgValue;
                model.EndingTotalValue = (int)(avgValue * endingTotal);

                var unitOrigin = acc.StockUnit;
                var unitConvert = matchedAcc?.StockUnit ?? "";
                int endingOrigin = matchedProduct.ConvertQuantity > 0 ? (int)(endingTotal / matchedProduct.ConvertQuantity) : 0;
                int endingConvert = matchedProduct.ConvertQuantity > 0 ? (int)(endingTotal % matchedProduct.ConvertQuantity) : 0;
                model.EndingString = $"{endingOrigin} {unitOrigin} & {endingConvert} {unitConvert}";
            }
            result.Add(model);
        }

        return result;
    }
    public async Task<InventoryStockResponse> GetConvertProductStockDataPaginator(int year, int page, int pageSize)
    {
        var result = new List<InventoryProductStockViewModel>();

        var oppositeDetails = _context.ConvertProducts
            .Select(x => new { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
            .ToList();

        var oppositeSet = new HashSet<string>(
            oppositeDetails.SelectMany(x => new[] { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
                           .Where(x => !string.IsNullOrEmpty(x))
        );

        var fullAccounts = await _context.GetChartOfAccount(year)
            .Where(x => (x.Classification == 2 || x.Classification == 3)
                        && !string.IsNullOrEmpty(x.ParentRef)
                        && x.Type > 4
                        && !x.HasDetails)
            .Select(x => new { x.WarehouseName, x.ParentRef, x.Code, x.Name, x.OpeningStockQuantityNB, x.OpeningDebitNB, x.StockUnit }).ToListAsync();

        var filteredAccounts = fullAccounts
            .Where(x => !oppositeSet.Contains(x.Code) && !oppositeSet.Contains(x.Name))
            .ToList();

        var codeList = fullAccounts.Select(x => x.Code).ToHashSet();

        var goodsList = await _context.Goods.ToListAsync();
        var ledgers = await _context.Ledgers
                            .Where(p => p.IsInternal == 3 &&
                                        (codeList.Contains(p.DebitDetailCodeFirst) || codeList.Contains(p.DebitDetailCodeSecond) ||
                                         codeList.Contains(p.CreditDetailCodeFirst) || codeList.Contains(p.CreditDetailCodeSecond)))
                            .ToListAsync();

        for (int i = 0; i < filteredAccounts.Count(); i++)
        {
            var acc = filteredAccounts[i];
            var parts = acc.ParentRef.Split(':');

            var acc_account = parts[0];
            string acc_detail1 = parts.Length > 1 ? parts[1] : acc.Code;
            string acc_detail2 = parts.Length > 1 ? acc.Code : null;

            var imports = ledgers.Where(l =>
                l.DebitCode == acc_account &&
                (string.IsNullOrEmpty(l.DebitWarehouseName) || l.DebitWarehouse == acc.WarehouseName) &&
                l.DebitDetailCodeFirst == acc_detail1 &&
                (string.IsNullOrEmpty(acc_detail2) || l.DebitDetailCodeSecond == acc_detail2)).ToList();

            var exports = ledgers.Where(l =>
                l.CreditCode == acc_account &&
                (string.IsNullOrEmpty(l.CreditWarehouseName) || l.DebitWarehouse == acc.WarehouseName) &&
                l.CreditDetailCodeFirst == acc_detail1 &&
                (string.IsNullOrEmpty(acc_detail2) || l.CreditDetailCodeSecond == acc_detail2)).ToList();

            double costPrice = 0;
            var validImports = imports.Where(l => l.Amount >= 0 && l.Quantity > 0).ToList();
            if (validImports.Any())
                costPrice = (validImports.Sum(l => l.Amount) + (acc.OpeningDebitNB ?? 0)) / (validImports.Sum(p => p.Quantity) + (acc.OpeningStockQuantityNB ?? 0));

            var endingStock = (acc.OpeningStockQuantityNB ?? 0) + imports.Sum(x => x.Quantity) - exports.Sum(x => x.Quantity);

            var matchedProduct = _context.ConvertProducts.FirstOrDefault(cp =>
                cp.Account == acc_account &&
                cp.Detail1 == acc_detail1 &&
                (string.IsNullOrEmpty(acc_detail2) || cp.Detail2 == acc_detail2));

            double convertAvgCost = (matchedProduct?.ConvertQuantity > 0) ? (costPrice / matchedProduct.ConvertQuantity) : 0;

            var model = new InventoryProductStockViewModel
            {
                Index = i + 1,
                Code = acc.Code,
                Name = acc.Name,
                OpeningStock = (int)(acc.OpeningStockQuantityNB ?? 0),
                ImportQuantity = (int)imports.Sum(x => x.Quantity),
                ExportQuantity = (int)exports.Sum(x => x.Quantity),
                EndingStock = (int)endingStock,
                CostPrice = (int)costPrice,
                EndingValue = convertAvgCost
            };

            if (matchedProduct != null)
            {
                var oppCode = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetail2 : matchedProduct.OppositeDetail1;
                var oppName = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetailName2 : matchedProduct.OppositeDetailName1;

                model.OppositeCode = oppCode;
                model.OppositeName = oppName;

                var oppImports = ledgers.Where(l =>
                    l.DebitCode == matchedProduct.Account &&
                    (string.IsNullOrEmpty(l.DebitWarehouse) || l.DebitWarehouse == matchedProduct.WarehouseName) &&
                    l.DebitDetailCodeFirst == matchedProduct.OppositeDetail1 &&
                    l.DebitDetailCodeSecond == matchedProduct.OppositeDetail2).ToList();

                var oppExports = ledgers.Where(l =>
                    l.CreditCode == matchedProduct.Account &&
                    (string.IsNullOrEmpty(l.CreditWarehouse) || l.CreditWarehouse == matchedProduct.WarehouseName) &&
                    l.CreditDetailCodeFirst == matchedProduct.OppositeDetail1 &&
                    l.CreditDetailCodeSecond == matchedProduct.OppositeDetail2).ToList();

                var matchedAcc = fullAccounts.FirstOrDefault(x =>
                    (x.Code == matchedProduct.OppositeDetail1 && x.Name == matchedProduct.OppositeDetailName1) ||
                    (x.Code == matchedProduct.OppositeDetail2 && x.Name == matchedProduct.OppositeDetailName2));

                double oppCost = 0;
                var validOppImports = oppImports.Where(l => l.Amount >= 0 && l.Quantity > 0).ToList();
                if (validOppImports.Any())
                    oppCost = (validOppImports.Sum(l => l.Amount) + (matchedAcc.OpeningDebitNB ?? 0)) / (validOppImports.Sum(l => l.Quantity) + (matchedAcc.OpeningStockQuantityNB ?? 0));

                double avgValue = oppCost == 0 ? convertAvgCost : (convertAvgCost + oppCost) / 2;
                int openingOpp = (int)(matchedAcc?.OpeningStockQuantityNB ?? 0);
                int oppEnding = (int)(openingOpp + oppImports.Sum(x => x.Quantity) - oppExports.Sum(x => x.Quantity));
                var endingTotal = oppEnding + (matchedProduct.ConvertQuantity * endingStock);
                model.OppositeOpeningStock = openingOpp;
                model.OppositeImportQuantity = (int)oppImports.Sum(x => x.Quantity);
                model.OppositeExportQuantity = (int)oppExports.Sum(x => x.Quantity);
                model.OppositeEndingStock = oppEnding;
                model.OppositeEndingCost = (int)oppCost;
                model.EndingAvgValue = (int)avgValue;
                model.EndingTotalValue = (int)(avgValue * (oppEnding + (matchedProduct.ConvertQuantity * endingStock)));
                var unitOrigin = acc.StockUnit;
                var unitConvert = matchedAcc != null ? matchedAcc.StockUnit : "";
                int endingOrigin = (int)endingTotal / matchedProduct.ConvertQuantity;
                int endingConvert = (int)endingTotal % matchedProduct.ConvertQuantity;
                model.EndingString = $"{endingOrigin} {unitOrigin} & {endingConvert} {unitConvert}";
            }
            result.Add(model);
        }
        var summary = new InventoryProductStockSummary
        {
            TotalRecords = result.Count(),
            TotalEndingValue = result.Sum(x => x.EndingTotalValue),
            TotalCostImport = result.Sum(x => x.CostPrice * x.ImportQuantity + x.OppositeImportQuantity * x.OppositeEndingCost)
        };
        var resultPaginator = new PagingResult<InventoryProductStockViewModel>
        {
            TotalItems = result.Count(),
            CurrentPage = page,
            PageSize = pageSize,
            Data = result.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
        };
        return new InventoryStockResponse
        {
            Items = resultPaginator,
            Summary = summary
        };
    }
    public async Task<List<InventoryProductStockViewModel>> GetConvertProductStockDataFilter(int year, DateTime? fromDate, DateTime? toDate, string warehouse, string productName)
    {
        var result = new List<InventoryProductStockViewModel>();

        // 1. Load ConvertProducts và build lookup
        var convertProducts = await _context.ConvertProducts.ToListAsync();
        var convertProductMap = convertProducts
            .GroupBy(x => $"{x.Account}_{x.Detail1}_{x.Detail2}")
            .ToDictionary(g => g.Key, g => g.First());

        // 2. Build HashSet để lọc các đối tượng đối ứng
        var oppositeSet = new HashSet<string>(
            convertProducts
                .SelectMany(x => new[] { x.OppositeDetail1, x.OppositeDetailName1, x.OppositeDetail2, x.OppositeDetailName2 })
                .Where(x => !string.IsNullOrEmpty(x))
        );

        // 3. Lấy danh sách tài khoản gốc
        var chartAccounts = await _context.GetChartOfAccount(year)
            .Where(x => (x.Classification == 2 || x.Classification == 3)
                        && !string.IsNullOrEmpty(x.ParentRef)
                        && x.Type > 4
                        && !x.HasDetails)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(productName))
        {
            var lowerProductName = productName.ToLower();
            chartAccounts = chartAccounts
                .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(lowerProductName))
                .ToList();
        }

        var codeSet = chartAccounts.Select(x => x.Code).ToHashSet();

        var filteredAccounts = chartAccounts
            .Where(x => !oppositeSet.Contains(x.Code) && !oppositeSet.Contains(x.Name))
            .ToList();

        // 1. Chuẩn bị mốc thời gian
        var yearStart = new DateTime(year, 1, 1);
        var fromDateOnly = fromDate?.Date ?? yearStart;
        var toDateOnly = toDate?.Date.AddDays(1);

        // 2. Truy xuất ledger 1 lần duy nhất trong năm
        var ledgersQuery = _context.Ledgers.Where(p => p.IsInternal == 3
            && p.OrginalBookDate >= yearStart
            && (toDateOnly == null || p.OrginalBookDate < toDateOnly));

        if (!string.IsNullOrEmpty(warehouse))
        {
            ledgersQuery = ledgersQuery.Where(p => p.CreditWarehouse == warehouse || p.DebitWarehouse == warehouse);
        }

        ledgersQuery = ledgersQuery.Where(p =>
            codeSet.Contains(p.DebitDetailCodeFirst) ||
            codeSet.Contains(p.DebitDetailCodeSecond) ||
            codeSet.Contains(p.CreditDetailCodeFirst) ||
            codeSet.Contains(p.CreditDetailCodeSecond));

        var ledgers = await ledgersQuery.ToListAsync();

        // 3. Tách ledger thành 2 phần: opening và trong kỳ
        var ledgersForOpening = ledgers
            .Where(p => p.OrginalBookDate < fromDateOnly)
            .ToList();

        var ledgersInPeriod = ledgers
            .Where(p => p.OrginalBookDate >= fromDateOnly)
            .ToList();

        // 4. Build map cho ledger trong kỳ
        var debitLedgerMap = ledgersInPeriod
            .GroupBy(l => $"{l.DebitCode}_{l.DebitDetailCodeFirst}_{l.DebitDetailCodeSecond}")
            .ToDictionary(g => g.Key, g => g.ToList());

        var creditLedgerMap = ledgersInPeriod
            .GroupBy(l => $"{l.CreditCode}_{l.CreditDetailCodeFirst}_{l.CreditDetailCodeSecond}")
            .ToDictionary(g => g.Key, g => g.ToList());

        // 6. Bắt đầu xử lý từng account
        for (int i = 0; i < filteredAccounts.Count; i++)
        {
            var acc = filteredAccounts[i];
            var parts = acc.ParentRef.Split(':');
            var acc_account = parts[0];
            var acc_detail1 = parts.Length > 1 ? parts[1] : acc.Code;
            var acc_detail2 = parts.Length > 1 ? acc.Code : null;

            var key = $"{acc_account}_{acc_detail1}_{acc_detail2}";
            debitLedgerMap.TryGetValue(key, out var imports);
            creditLedgerMap.TryGetValue(key, out var exports);

            double openingQty = acc.OpeningStockQuantityNB ?? 0;
            double openingDebit = acc.OpeningDebitNB ?? 0;

            double totalImportQty = imports?.Sum(l => l.Quantity) ?? 0;
            double totalImportAmt = imports?.Where(l => l.Amount >= 0 && l.Quantity > 0).Sum(l => l.Amount) ?? 0;
            double validImportQty = imports?.Where(l => l.Amount >= 0 && l.Quantity > 0).Sum(l => l.Quantity) ?? 0;
            double totalExportQty = exports?.Sum(l => l.Quantity) ?? 0;
            double debitAmount = 0, debitQty = 0;

            double costPrice = validImportQty + openingQty > 0
                ? (totalImportAmt + openingDebit) / (validImportQty + openingQty)
                : 0;

            if (fromDateOnly > yearStart)
            {
                debitQty = openingQty + ledgersForOpening
                    .Where(p => p.DebitCode == acc_account && p.DebitDetailCodeFirst == acc_detail1
                    && (acc_detail2 == null || p.DebitDetailCodeSecond == acc_detail2))
                    .Sum(p => p.Quantity);

                debitAmount = openingDebit + ledgersForOpening
                    .Where(p => p.DebitCode == acc_account && p.DebitDetailCodeFirst == acc_detail1
                    && (acc_detail2 == null || p.DebitDetailCodeSecond == acc_detail2))
                    .Sum(p => p.Amount)
                    //-
                    //ledgersForOpening
                    //.Where(p => p.CreditCode == acc_account && p.CreditDetailCodeFirst == acc_detail1
                    //&& (acc_detail2 == null || p.CreditDetailCodeSecond == acc_detail2)
                    //&& p.Amount >= 0 && p.Quantity >= 0)
                    //.Sum(p => p.Amount)
                    ;

                costPrice = validImportQty + debitQty > 0 ? (totalImportAmt + debitAmount) / (totalImportQty + debitQty): 0;

                openingQty += ledgersForOpening
                    .Where(p => p.DebitCode == acc_account && p.DebitDetailCodeFirst == acc_detail1
                    && (acc_detail2 == null || p.DebitDetailCodeSecond == acc_detail2)
                    && p.Amount >= 0 && p.Quantity >= 0)
                    .Sum(p => p.Quantity)
                    -
                    ledgersForOpening
                    .Where(p => p.CreditCode == acc_account && p.CreditDetailCodeFirst == acc_detail1
                    && (acc_detail2 == null || p.CreditDetailCodeSecond == acc_detail2)
                    && p.Amount >= 0 && p.Quantity >= 0)
                    .Sum(p => p.Quantity)
                    ;
            }

            double endingStock = openingQty + totalImportQty - totalExportQty;
            //double importCost = validImportQty > 0 ? totalImportAmt / validImportQty : 0;
            // ConvertProduct tra từ Dictionary
            convertProductMap.TryGetValue(key, out var matchedProduct);
            double convertAvgCost = (matchedProduct?.ConvertQuantity > 0) ? (costPrice / matchedProduct.ConvertQuantity) : 0;

            var model = new InventoryProductStockViewModel
            {
                Index = i + 1,
                Code = acc.Code,
                Name = acc.Name,
                OpeningStock = (int)openingQty,
                ImportQuantity = (int)totalImportQty,
                ExportQuantity = (int)totalExportQty,
                EndingStock = (int)endingStock,
                CostPrice = (int)costPrice,
                EndingValue = convertAvgCost
            };

            if (matchedProduct != null)
            {
                var oppCode = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetail2 : matchedProduct.OppositeDetail1;
                var oppName = !string.IsNullOrEmpty(matchedProduct.OppositeDetail2) ? matchedProduct.OppositeDetailName2 : matchedProduct.OppositeDetailName1;

                model.OppositeCode = oppCode;
                model.OppositeName = oppName;

                var oppKey = $"{matchedProduct.Account}_{matchedProduct.OppositeDetail1}_{matchedProduct.OppositeDetail2}";
                debitLedgerMap.TryGetValue(oppKey, out var oppImports);
                creditLedgerMap.TryGetValue(oppKey, out var oppExports);

                var matchedAcc = chartAccounts.FirstOrDefault(x =>
                    (x.Code == matchedProduct.OppositeDetail1 && x.Name == matchedProduct.OppositeDetailName1) ||
                    (x.Code == matchedProduct.OppositeDetail2 && x.Name == matchedProduct.OppositeDetailName2));

                double oppOpeningQty = matchedAcc?.OpeningStockQuantityNB ?? 0;
                double oppOpeningDebit = matchedAcc?.OpeningDebitNB ?? 0;

                if (fromDateOnly > yearStart)
                {
                    oppOpeningQty += ledgersForOpening
                        .Where(p => p.DebitCode == matchedProduct.Account
                            && p.DebitDetailCodeFirst == matchedProduct.OppositeDetail1
                            && (string.IsNullOrEmpty(matchedProduct.OppositeDetail2) || p.DebitDetailCodeSecond == matchedProduct.OppositeDetail2))
                        .Sum(p => p.Quantity)
                        -
                        ledgersForOpening
                        .Where(p => p.CreditCode == matchedProduct.Account
                            && p.CreditDetailCodeFirst == matchedProduct.OppositeDetail1
                            && (string.IsNullOrEmpty(matchedProduct.OppositeDetail2) || p.CreditDetailCodeSecond == matchedProduct.OppositeDetail2))
                        .Sum(p => p.Quantity);

                    oppOpeningDebit += ledgersForOpening
                        .Where(p => p.DebitCode == matchedProduct.Account
                            && p.DebitDetailCodeFirst == matchedProduct.OppositeDetail1
                            && (string.IsNullOrEmpty(matchedProduct.OppositeDetail2) || p.DebitDetailCodeSecond == matchedProduct.OppositeDetail2))
                        .Sum(p => p.Amount)
                        -
                        ledgersForOpening
                        .Where(p => p.CreditCode == matchedProduct.Account
                            && p.CreditDetailCodeFirst == matchedProduct.OppositeDetail1
                            && (string.IsNullOrEmpty(matchedProduct.OppositeDetail2) || p.CreditDetailCodeSecond == matchedProduct.OppositeDetail2))
                        .Sum(p => p.Amount);
                }

                double oppImportQty = oppImports?.Sum(x => x.Quantity) ?? 0;
                double oppImportAmt = oppImports?.Where(x => x.Amount >= 0 && x.Quantity > 0).Sum(x => x.Amount) ?? 0;
                double oppValidQty = oppImports?.Where(x => x.Amount >= 0 && x.Quantity > 0).Sum(x => x.Quantity) ?? 0;
                double oppExportQty = oppExports?.Sum(x => x.Quantity) ?? 0;

                double oppCost = (oppValidQty + oppOpeningQty > 0)
                    ? (oppImportAmt + oppOpeningDebit) / (oppValidQty + oppOpeningQty)
                    : 0;

                double avgValue = oppCost == 0 ? convertAvgCost : (convertAvgCost + oppCost) / 2;

                int oppEnding = (int)(oppOpeningQty + oppImportQty - oppExportQty);
                int endingTotal = oppEnding + (int)(matchedProduct.ConvertQuantity * endingStock);

                model.OppositeOpeningStock = (int)oppOpeningQty;
                model.OppositeImportQuantity = (int)oppImportQty;
                model.OppositeExportQuantity = (int)oppExportQty;
                model.OppositeEndingStock = oppEnding;
                model.OppositeEndingCost = (int)oppCost;
                model.EndingAvgValue = (int)avgValue;
                model.EndingTotalValue = (int)(avgValue * endingTotal);

                var unitOrigin = acc.StockUnit;
                var unitConvert = matchedAcc?.StockUnit ?? "";
                int endingOrigin = matchedProduct.ConvertQuantity > 0 ? (int)(endingTotal / matchedProduct.ConvertQuantity) : 0;
                int endingConvert = matchedProduct.ConvertQuantity > 0 ? (int)(endingTotal % matchedProduct.ConvertQuantity) : 0;
                model.EndingString = $"{endingOrigin} {unitOrigin} & {endingConvert} {unitConvert}";
            }
            else
            {
                convertAvgCost = costPrice;
                model.EndingAvgValue = (int)convertAvgCost;
                model.EndingTotalValue = (int)(convertAvgCost * endingStock);

                var unitOrigin = acc.StockUnit;
                model.EndingString = $"{endingStock} {unitOrigin}";
            }
                result.Add(model);
        }

        return result;
    }
}