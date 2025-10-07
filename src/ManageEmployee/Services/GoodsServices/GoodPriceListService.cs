using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.GoodsServices;
public class GoodPriceListService: IGoodPriceListService
{
    private readonly ApplicationDbContext _context;
    private readonly ILedgerService _ledgerServices;

    public GoodPriceListService(ApplicationDbContext context, ILedgerService ledgerServices)
    {
        _context = context;
        _ledgerServices = ledgerServices;
    }
    public async Task CopyPriceList(CopyPriceListRequest request)
    {
        var listGood = await _context.Goods.AsNoTracking().Where(x => x.PriceList == request.PriceListFrom
            && (request.listId == null || !request.listId.Any() || request.listId.Contains(x.Id))
            ).ToListAsync();

        var itemChecks = await _context.Goods.Where(x => x.PriceList == request.PriceListTo).ToListAsync();
        _context.ChangeTracker.AutoDetectChangesEnabled = false;
        List<Goods> listadd = new List<Goods>();
        foreach (var item in listGood)
        {
            var itemCheck = itemChecks.Find(x => x.PriceList == request.PriceListTo && x.Account == item.Account
           && x.Detail1 == item.Detail1 && x.Detail2 == item.Detail2 && x.Warehouse == item.Warehouse);
            if (itemCheck != null)
            {
                if (request.TypeMoney == 1)
                {
                    switch (request.Type)
                    {
                        case 0:
                            itemCheck.SalePrice = item.SalePrice + (request.Cash ?? 0);
                            itemCheck.Price = item.Price + (request.Cash ?? 0);
                            itemCheck.DiscountPrice = item.DiscountPrice + (request.Cash ?? 0);
                            break;

                        case 1:
                            itemCheck.SalePrice = item.SalePrice - (request.Cash ?? 0);
                            itemCheck.Price = item.Price - (request.Cash ?? 0);
                            itemCheck.DiscountPrice = item.DiscountPrice - (request.Cash ?? 0);
                            break;

                        case 2:
                            itemCheck.SalePrice = request.Cash ?? 0;
                            itemCheck.Price = request.Cash ?? 0;
                            itemCheck.DiscountPrice = request.Cash ?? 0;
                            break;
                    }
                }
                else
                {
                    switch (request.Type)
                    {
                        case 0:
                            itemCheck.SalePrice = item.SalePrice * (1 + (request.Percent ?? 0) / 100);
                            itemCheck.Price = item.Price * (1 + (request.Percent ?? 0) / 100);
                            itemCheck.DiscountPrice = item.DiscountPrice * (1 + (request.Percent ?? 0) / 100);
                            break;

                        case 1:
                            itemCheck.SalePrice = item.SalePrice * (1 - (request.Percent ?? 0) / 100);
                            itemCheck.Price = item.Price * (1 - (request.Percent ?? 0) / 100);
                            itemCheck.DiscountPrice = item.DiscountPrice * (1 - (request.Percent ?? 0) / 100);
                            break;

                        case 2:
                            itemCheck.SalePrice = item.SalePrice * ((request.Percent ?? 0) / 100);
                            itemCheck.Price = item.Price * ((request.Percent ?? 0) / 100);
                            itemCheck.DiscountPrice = item.DiscountPrice * ((request.Percent ?? 0) / 100);
                            break;
                    }
                }
            }
            else
            {
                item.Id = 0;
                item.CreateAt = DateTime.Now;
                item.UserCreated = request.UserCreated;
                item.PriceList = request.PriceListTo;
                if (request.TypeMoney == 1)
                {
                    switch (request.Type)
                    {
                        case 0:
                            item.SalePrice = item.SalePrice + (request.Cash ?? 0);
                            item.Price = item.Price + (request.Cash ?? 0);
                            item.DiscountPrice = item.DiscountPrice + (request.Cash ?? 0);
                            break;

                        case 1:
                            item.SalePrice = item.SalePrice - (request.Cash ?? 0);
                            item.Price = item.Price - (request.Cash ?? 0);
                            item.DiscountPrice = item.DiscountPrice - (request.Cash ?? 0);
                            break;

                        case 2:
                            item.SalePrice = request.Cash ?? 0;
                            item.Price = request.Cash ?? 0;
                            item.DiscountPrice = request.Cash ?? 0;
                            break;
                    }
                }
                else
                {
                    switch (request.Type)
                    {
                        case 0:
                            item.SalePrice = item.SalePrice * (1 + (request.Percent ?? 0) / 100);
                            item.Price = item.Price * (1 + (request.Percent ?? 0) / 100);
                            item.DiscountPrice = item.DiscountPrice * (1 + (request.Percent ?? 0) / 100);
                            break;

                        case 1:
                            item.SalePrice = item.SalePrice * (1 - (request.Percent ?? 0) / 100);
                            item.Price = item.Price * (1 - (request.Percent ?? 0) / 100);
                            item.DiscountPrice = item.DiscountPrice * (1 - (request.Percent ?? 0) / 100);
                            break;

                        case 2:
                            item.SalePrice = item.SalePrice * ((request.Percent ?? 0) / 100);
                            item.Price = item.Price * ((request.Percent ?? 0) / 100);
                            item.DiscountPrice = item.DiscountPrice * ((request.Percent ?? 0) / 100);
                            break;
                    }
                }
                listadd.Add(item);
            }
        }

        _context.Goods.UpdateRange(itemChecks);
        await _context.Goods.AddRangeAsync(listadd);
        _context.ChangeTracker.DetectChanges();
        await _context.SaveChangesAsync();
        _context.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public async Task UpdatePriceList(UpdatePriceListRequest request, int year)
    {
        var listGood = await _context.Goods.AsNoTracking().Where(x => x.Status == 1 && x.PriceList == request.PriceList
            && (request.listId == null || request.listId.Contains(x.Id))
            )
             .GroupBy(m => new { m.Account, m.Detail1, m.Detail2, m.Warehouse })
            .Select(group => group.First())
            .ToListAsync();
        var taxRates = new List<TaxRate>();
        if (request.PriceTo == 0)
        {
            taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
            taxRates = taxRates.Where(x => x.Code.Contains("R")).ToList();
        }
        foreach (var item in listGood)
        {
            double priceUpdate = 0;
            switch (request.PriceFrom)
            {
                case 0:
                    if (request.TypeMoney == 1)
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = item.SalePrice + (request.Cash ?? 0);
                                break;

                            case 1:
                                priceUpdate = item.SalePrice - (request.Cash ?? 0);
                                break;

                            case 2:
                                priceUpdate = request.Cash ?? 0;
                                break;
                        }
                    else
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = item.SalePrice * (1 + (request.Percent ?? 0) / 100);
                                break;

                            case 1:
                                priceUpdate = item.SalePrice * (1 - (request.Percent ?? 0) / 100);
                                break;

                            case 2:
                                priceUpdate = item.SalePrice * ((request.Percent ?? 0) / 100);
                                break;
                        }
                    break;

                case 1:
                    if (request.TypeMoney == 1)
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = item.Price + (request.Cash ?? 0);
                                break;

                            case 1:
                                priceUpdate = item.SalePrice - (request.Cash ?? 0);
                                break;

                            case 2:
                                priceUpdate = request.Cash ?? 0;
                                break;
                        }
                    else
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = item.Price * (1 + (request.Percent ?? 0) / 100);
                                break;

                            case 1:
                                priceUpdate = item.Price * (1 - (request.Percent ?? 0) / 100);
                                break;

                            case 2:
                                priceUpdate = item.Price * ((request.Percent ?? 0) / 100);
                                break;
                        }
                    break;

                case 2:
                    if (request.TypeMoney == 1)
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = item.DiscountPrice + (request.Cash ?? 0);
                                break;

                            case 1:
                                priceUpdate = item.DiscountPrice - (request.Cash ?? 0);
                                break;

                            case 2:
                                priceUpdate = request.Cash ?? 0;
                                break;
                        }
                    else
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = item.DiscountPrice * (1 + (request.Percent ?? 0) / 100);
                                break;

                            case 1:
                                priceUpdate = item.DiscountPrice * (1 - (request.Percent ?? 0) / 100);
                                break;

                            case 2:
                                priceUpdate = item.DiscountPrice * ((request.Percent ?? 0) / 100);
                                break;
                        }
                    break;

                case 3:
                    var taxRate = taxRates.Find(x => x.Id == item.TaxRateId);
                    var taxVat = item.SalePrice * (taxRate?.Percent ?? 0) / 100;
                    if (request.TypeMoney == 1)
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = taxVat + (request.Cash ?? 0);
                                break;

                            case 1:
                                priceUpdate = taxVat - (request.Cash ?? 0);
                                break;

                            case 2:
                                priceUpdate = request.Cash ?? 0;
                                break;
                        }
                    else
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = taxVat * (1 + (request.Percent ?? 0) / 100);
                                break;

                            case 1:
                                priceUpdate = taxVat * (1 - (request.Percent ?? 0) / 100);
                                break;

                            case 2:
                                priceUpdate = taxVat * ((request.Percent ?? 0) / 100);
                                break;
                        }
                    break;

                case 4:
                    double dUnitPriceAvg = await _ledgerServices.TinhGiaXuatKho(item.Account, item.Detail1, item.Detail2,
                                        item.Warehouse, DateTime.Today, year, 0);
                    if (request.TypeMoney == 1)
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = dUnitPriceAvg + (request.Cash ?? 0);
                                break;

                            case 1:
                                priceUpdate = dUnitPriceAvg - (request.Cash ?? 0);
                                break;

                            case 2:
                                priceUpdate = request.Cash ?? 0;
                                break;
                        }
                    else
                        switch (request.Type)
                        {
                            case 0:
                                priceUpdate = dUnitPriceAvg * (1 + (request.Percent ?? 0) / 100);
                                break;

                            case 1:
                                priceUpdate = dUnitPriceAvg * (1 - (request.Percent ?? 0) / 100);
                                break;

                            case 2:
                                priceUpdate = dUnitPriceAvg * ((request.Percent ?? 0) / 100);
                                break;
                        }
                    break;
            }

            switch (request.PriceTo)
            {
                case 0:
                    item.SalePrice = priceUpdate;
                    break;

                case 1:
                    item.Price = priceUpdate;
                    break;

                case 2:
                    item.DiscountPrice = priceUpdate;
                    break;

                case 3:
                    break;
            }
        }

        _context.Goods.UpdateRange(listGood);
        await _context.SaveChangesAsync();
    }

    public async Task<string> ExportGoodComparePrice(ComparePriceListRequest param)
    {
        try
        {
            if (param.PriceLists.Count == 0)
            {
                param.PriceLists = await _context.Categories.Where(x => x.Type == (int)CategoryEnum.PriceList && !x.IsDeleted).Select(x => x.Code).Distinct().ToListAsync();
            }

            var datas = await GoodComparePrice(param, true);
            string sTenFile = "GoodComparePrice.xlsx";
            int nCol = 6;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + sTenFile);
            using (FileStream templateDocumentStream = File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 5, nRow = 7;
                    int i = 0;

                    foreach (var priceList in param.PriceLists)
                    {
                        worksheet.Cells[5, nCol].Value = priceList;
                        worksheet.Cells[5, nCol, 5, nCol + 2].Merge = true;

                        worksheet.Cells[6, nCol].Value = "Giá bán";
                        worksheet.Cells[6, nCol + 1].Value = "Thuế VAT";
                        worksheet.Cells[6, nCol + 2].Value = "Tổng tiền";
                        nCol = nCol + 3;
                    }

                    foreach (var data in datas)
                    {
                        i++;
                        worksheet.Cells[nRow, 1].Value = i;
                        worksheet.Cells[nRow, 2].Value = data.Image1;
                        worksheet.Cells[nRow, 3].Value = data.Code;
                        worksheet.Cells[nRow, 4].Value = data.Name;
                        worksheet.Cells[nRow, 5].Value = data.WarehouseName;
                        nCol = 6;
                        foreach (var pricelist in data.listItem)
                        {
                            worksheet.Cells[nRow, nCol].Value = pricelist.SalePrice;
                            worksheet.Cells[nRow, nCol + 1].Value = pricelist.TaxVAT;
                            worksheet.Cells[nRow, nCol + 2].Value = pricelist.Amount;
                            nCol = nCol + 3;
                        }
                        nRow++;
                    }
                    nRow--;
                    nCol--;

                    worksheet.Cells[nRowBegin, 6, nRow, nCol].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, nRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "GoodComparePrice");
                }
            }
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    private async Task<List<GoodComparePriceResult>> GoodComparePrice(ComparePriceListRequest request, bool isExport = false)
    {
        var query = from g in _context.Goods
                        //join gCompare in _context.Goods on g.Account equals gCompare.Account
                    where g.PriceList == "BGC"
                    && (string.IsNullOrEmpty(request.Account) || g.Account == request.Account)
                    && (string.IsNullOrEmpty(request.Detail1) || g.Detail1 == request.Detail1)
                    && (string.IsNullOrEmpty(request.Position) || g.Position == request.Position)
                    && (string.IsNullOrEmpty(request.Warehouse) || g.Warehouse == request.Warehouse)
                    && (string.IsNullOrEmpty(request.MenuType) || g.MenuType == request.MenuType)
                    && (string.IsNullOrEmpty(request.GoodType) || g.GoodsType == request.GoodType)
                    && (request.FromDifferentSalePrice == 0 || g.SalePrice + g.SalePrice * (request.FromDifferentSalePrice ?? 0) > 0)
                    && (request.ToDifferentSalePrice == 0 || g.SalePrice + g.SalePrice * (request.ToDifferentSalePrice ?? 0) < 0)
                    select g
                         ;

        List<GoodComparePriceResult> listOut = new List<GoodComparePriceResult>();
        List<Goods> listGood;
        if (!isExport)
            listGood = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        else
            listGood = await query.ToListAsync();

        var taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
        foreach (var good in listGood)
        {
            GoodComparePriceResult item = new GoodComparePriceResult();
            item.Code = GoodNameGetter.GetCodeFromGood(good);
            item.Name = GoodNameGetter.GetNameFromGood(good);
            item.Image1 = good.Image1;
            item.WarehouseName = good.WarehouseName;
            item.listItem = new List<GoodComparePriceItem>();
            double differentSalePrice = 0;
            int i = 0;
            foreach (var priceList in request.PriceLists)
            {
                GoodComparePriceItem child = new GoodComparePriceItem();
                child.Code = priceList;
                if (priceList == "BGC")
                {
                    var taxRate = taxRates.Find(x => x.Id == good.TaxRateId);

                    child.SalePrice = good.SalePrice;
                    child.TaxVAT = (taxRate?.Percent ?? 0) * good.SalePrice / 100;
                    child.Amount = good.SalePrice + (child.TaxVAT ?? 0);
                }
                else
                {
                    Goods goodBGDL = await _context.Goods.FirstOrDefaultAsync(x => x.PriceList == priceList && x.Account == good.Account
                                        && x.Detail1 == good.Detail1 && x.Detail2 == good.Detail2 && x.Warehouse == good.Warehouse);
                    if (goodBGDL != null)
                    {
                        var taxRate = taxRates.Find(x => x.Id == goodBGDL.TaxRateId);

                        child.SalePrice = goodBGDL.SalePrice;
                        child.TaxVAT = goodBGDL.SalePrice * (taxRate?.Percent ?? 0);
                        child.Amount = goodBGDL.SalePrice + (child.TaxVAT ?? 0);
                    }
                }
                if (i == 0)
                {
                    child.DifferentSalePrice = 0;
                    differentSalePrice = child.SalePrice;
                }
                else
                {
                    if (differentSalePrice == 0)
                    {
                        child.DifferentSalePrice = 0;
                    }
                    else
                    {
                        child.DifferentSalePrice = Math.Round((child.SalePrice - differentSalePrice) / differentSalePrice * 100, 2);
                    }
                }
                item.listItem.Add(child);
                i++;
            }
            listOut.Add(item);
        }
        return listOut;
    }


    public async Task<PagingResult<GoodComparePriceResult>> GetPagingGoodComparePrice(ComparePriceListRequest request)
    {

        if (request.PriceLists.Count == 0)
        {
            request.PriceLists = await _context.Categories.Where(x => x.Type == 0 && !x.IsDeleted).Select(x => x.Code).Distinct().ToListAsync();
        }

        return new PagingResult<GoodComparePriceResult>()
        {
            Data = await GoodComparePrice(request),
            PageSize = request.PageSize,
            TotalItems = await _context.Goods.Where(x => x.PriceList == "BGC").CountAsync(),
            CurrentPage = request.Page
        };
    }

    public async Task<List<Goods>> GetPriceByPriceCode(string priceCode,
        List<GoodCodeModel> goodCodes)
    {
        goodCodes ??= new List<GoodCodeModel>();
        var goods = await _context.Goods.Where(x =>
            x.PriceList == priceCode
            && x.Detail2 != null
            && x.Detail1 != null
        ).ToListAsync();

        goods = goods.Where(x => goodCodes.Exists(code => code.Detail1 == x.Detail1 && code.Detail2 == x.Detail2)).ToList();

        return goods;
    }

}
