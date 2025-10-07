using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.GoodsServices
{
    public class GoodExporter : IGoodExporter
    {
        private readonly ApplicationDbContext _context;
        private readonly IGoodsService _goodsService;

        public GoodExporter(ApplicationDbContext context, IGoodsService goodsService)
        {
            _context = context;
            _goodsService = goodsService;
        }
        public async Task<string> GetExcelReport(SearchViewModel param, bool isManager)
        {
            var data = await _goodsService.GetAll_Common(param).ToListAsync();

            if (data.Count > 0)
            {
                if (isManager)
                    return await ExportExcelManager(data);

                return await ExportExcel(data);
            }
            return string.Empty;
        }

        private async Task<string> ExportExcel(List<GoodsExportlModel> goods)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/BangKeHangHoa.xlsx");
            MemoryStream stream = new MemoryStream();
            List<Category> lstGroupType = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();

            using (FileStream templateDocumentStream = File.OpenRead(path))
            {
                using ExcelPackage package = new ExcelPackage(templateDocumentStream);
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 7;
                int iRow = nRowBegin;
                var listMenuType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.GoodGroup).Take(10).ToList();
                var listPriceType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.PriceList).Take(4).ToList();
                listPriceType.Add(lstGroupType.Find(t => t.Code == goods[0].PriceList && t.Type == (int)CategoryEnum.PriceList));
                var listGoodsType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.GoodsType2).Take(10).ToList();
                var listPositionType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.Position).Take(10).ToList();
                var listMenuWeb = lstGroupType.Where(x => x.Type == (int)CategoryEnum.MenuWeb).Take(10).ToList();
                var taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
                for (int i = 0; i < goods.Count; i++)
                {
                    Goods item = goods[i];

                    string strMenuTypeName = listMenuType.Find(t => t.Code == item.MenuType)?.Name;
                    string strPriceListName = lstGroupType.Find(t => t.Code == item.PriceList && t.Type == (int)CategoryEnum.PriceList)?.Name;
                    string strGoodsTypeName = listGoodsType.Find(t => t.Code == item.GoodsType && t.Type == (int)CategoryEnum.GoodsType2)?.Name;
                    string strPositionName = listPositionType.Find(t => t.Code == item.Position)?.Name;

                    worksheet.Cells[iRow, 1].Value = string.IsNullOrEmpty(item.Detail2) ? item.Detail1 : item.Detail2;
                    worksheet.Cells[iRow, 2].Value = string.IsNullOrEmpty(item.DetailName2) ? item.DetailName1 : item.DetailName2;
                    worksheet.Cells[iRow, 3].Value = string.IsNullOrEmpty(item.Account) ? null : item.Account;
                    worksheet.Cells[iRow, 4].Value = string.IsNullOrEmpty(item.AccountName) ? null : item.AccountName;
                    worksheet.Cells[iRow, 5].Value = string.IsNullOrEmpty(item.Detail1) ? null : item.Detail1;
                    worksheet.Cells[iRow, 6].Value = string.IsNullOrEmpty(item.DetailName1) ? null : item.DetailName1;

                    worksheet.Cells[iRow, 7].Value = string.IsNullOrEmpty(item.Detail2) ? null : item.Detail2;
                    worksheet.Cells[iRow, 8].Value = string.IsNullOrEmpty(item.DetailName2) ? null : item.DetailName2;

                    worksheet.Cells[iRow, 9].Value = item.Warehouse;
                    worksheet.Cells[iRow, 10].Value = item.WarehouseName;

                    worksheet.Cells[iRow, 11].Value = item.Price;
                    worksheet.Cells[iRow, 12].Value = item.SalePrice;

                    var taxRate = taxRates.Find(x => x.Id == item.TaxRateId);
                    worksheet.Cells[iRow, 13].Value = $"{taxRate?.Name}-{taxRate?.Percent.ToString()}%";
                    worksheet.Cells[iRow, 14].Value = item.DiscountPrice;

                    worksheet.Cells[iRow, 15].Value = strMenuTypeName;

                    worksheet.Cells[iRow, 16].Value = strPriceListName;

                    worksheet.Cells[iRow, 17].Value = strGoodsTypeName;

                    worksheet.Cells[iRow, 18].Value = strPositionName;

                    worksheet.Cells[iRow, 20].Value = item.Image1;
                    worksheet.Cells[iRow, 21].Value = item.Image2;
                    worksheet.Cells[iRow, 22].Value = item.Image3;
                    worksheet.Cells[iRow, 23].Value = item.Image4;
                    worksheet.Cells[iRow, 24].Value = item.Image5;
                    iRow++;
                }
                iRow--;
                if (listMenuType.Count > 0)
                {
                    var menu = worksheet.Cells[nRowBegin, 15, iRow, 15].DataValidation.AddListDataValidation();
                    foreach (var itemFor in listMenuType)
                    {
                        menu.Formula.Values.Add(itemFor.Name);
                    }
                }
                if (listPriceType.Count > 0)
                {
                    var price = worksheet.Cells[nRowBegin, 16, iRow, 16].DataValidation.AddListDataValidation();
                    foreach (var itemFor in listPriceType)
                    {
                        price.Formula.Values.Add(itemFor.Name);
                    }
                }
                if (listGoodsType.Count > 0)
                {
                    var goodstype = worksheet.Cells[nRowBegin, 17, iRow, 17].DataValidation.AddListDataValidation();
                    foreach (var itemFor in listGoodsType)
                    {
                        goodstype.Formula.Values.Add(itemFor.Name);
                    }
                }
                if (listPositionType.Count > 0)
                {
                    var position = worksheet.Cells[nRowBegin, 18, iRow, 18].DataValidation.AddListDataValidation();
                    foreach (var itemFor in listPositionType)
                    {
                        position.Formula.Values.Add(itemFor.Name);
                    }
                }
                if (listMenuWeb.Count > 0)
                {
                    var menuWeb = worksheet.Cells[nRowBegin, 19, iRow, 19].DataValidation.AddListDataValidation();
                    foreach (var itemFor in listMenuWeb)
                    {
                        menuWeb.Formula.Values.Add(itemFor.Name);
                    }
                }
                if (taxRates.Count > 0)
                {
                    var taxRate = worksheet.Cells[nRowBegin, 13, iRow, 13].DataValidation.AddListDataValidation();
                    foreach (var itemFor in taxRates)
                    {
                        taxRate.Formula.Values.Add($"{itemFor.Name}-{itemFor.Percent}%");
                    }
                }
                if (iRow >= nRowBegin)
                {
                    worksheet.Cells[nRowBegin, 11, iRow, 14].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, iRow, 24].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                package.SaveAs(stream);
                return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "BangKeHangHoa");
            }
        }

        private async Task<string> ExportExcelManager(List<GoodsExportlModel> goods)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/DanhSachHangHoa.xlsx");
            MemoryStream stream = new MemoryStream();
            List<Category> lstGroupType = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            using (FileStream templateDocumentStream = File.OpenRead(path))
            {
                using ExcelPackage package = new ExcelPackage(templateDocumentStream);
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 7;
                int iRow = nRowBegin;
                var listGoodsType = await _context.Categories.Where(x => !x.IsDeleted && x.Type == (int)CategoryEnum.GoodsType2).ToListAsync();
                //var listPriceType = lstGroupType.Where(x => x.Type == (int)CategoryEnum.PriceList).Take(4).ToList();
                var taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();

                for (int i = 0; i < goods.Count; i++)
                {
                    Goods item = goods[i];
                    //string strPriceListName = lstGroupType.Find(t => t.Code == item.PriceList && t.Type == (int)CategoryEnum.PriceList)?.Name;

                    worksheet.Cells[iRow, 1].Value = item.Image1;
                    worksheet.Cells[iRow, 2].Value = string.IsNullOrEmpty(item.Account) ? null : item.Account;
                    worksheet.Cells[iRow, 3].Value = string.IsNullOrEmpty(item.AccountName) ? null : item.AccountName;
                    worksheet.Cells[iRow, 4].Value = string.IsNullOrEmpty(item.Detail1) ? null : item.Detail1;
                    worksheet.Cells[iRow, 5].Value = string.IsNullOrEmpty(item.DetailName1) ? null : item.DetailName1;
                    worksheet.Cells[iRow, 6].Value = string.IsNullOrEmpty(item.Detail2) ? null : item.Detail2;
                    worksheet.Cells[iRow, 7].Value = string.IsNullOrEmpty(item.DetailName2) ? null : item.DetailName2;
                    worksheet.Cells[iRow, 8].Value = string.IsNullOrEmpty(item.Warehouse) ? null : item.Warehouse;
                    worksheet.Cells[iRow, 9].Value = string.IsNullOrEmpty(item.WarehouseName) ? null : item.WarehouseName;
                    worksheet.Cells[iRow, 10].Value = item.GoodsType;
                    worksheet.Cells[iRow, 11].Value = item.MinStockLevel;
                    worksheet.Cells[iRow, 12].Value = item.MaxStockLevel;
                    worksheet.Cells[iRow, 13].Value = item.Net;
                    worksheet.Cells[iRow, 14].Value = item.SalePrice;
                    //worksheet.Cells[iRow, 15].Value = strPriceListName;
                    var taxRate = taxRates.Find(x => x.Id == item.TaxRateId);
                    worksheet.Cells[iRow, 15].Value = $"{taxRate?.Name}-{taxRate?.Percent.ToString()}%";
                    worksheet.Cells[iRow, 16].Value = item.Status == 1 ? "Đang kinh doanh" : "Ngừng kinh doanh";
                    iRow++;
                }
                iRow--;
                if (listGoodsType.Count > 0)
                {
                    var goodstype = worksheet.Cells[nRowBegin, 8, iRow, 8].DataValidation.AddListDataValidation();
                    foreach (var itemFor in listGoodsType)
                    {
                        goodstype.Formula.Values.Add(itemFor.Name);
                    }
                }
                //if (listPriceType.Count > 0)
                //{
                //    var price = worksheet.Cells[nRowBegin, 15, iRow, 15].DataValidation.AddListDataValidation();
                //    foreach (var itemFor in listPriceType)
                //    {
                //        price.Formula.Values.Add(itemFor.Name);
                //    }
                //}
                if (taxRates.Count > 0)
                {
                    var taxRate = worksheet.Cells[nRowBegin, 15, iRow, 15].DataValidation.AddListDataValidation();
                    foreach (var itemFor in taxRates)
                    {
                        taxRate.Formula.Values.Add($"{itemFor.Name}-{itemFor.Percent.ToString()}%");
                    }
                }
                var status = worksheet.Cells[nRowBegin, 16, iRow, 16].DataValidation.AddListDataValidation();
                status.Formula.Values.Add("Đang kinh doanh");
                status.Formula.Values.Add("Ngừng kinh doanh");

                int nCol = 16;
                if (iRow >= nRowBegin)
                {
                    worksheet.Cells[nRowBegin, 11, iRow, 16].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                package.SaveAs(stream);
                return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "DanhSachHangHoa");
            }
        }


    }
}
