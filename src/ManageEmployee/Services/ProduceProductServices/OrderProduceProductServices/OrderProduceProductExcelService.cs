using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.ProduceProductServices.OrderProduceProductServices;

public class OrderProduceProductExcelService : IOrderProduceProductExcelService
{
    private readonly ApplicationDbContext _context;
    private readonly IOrderProduceProductService _orderProduceProductService;

    public OrderProduceProductExcelService(ApplicationDbContext context, IOrderProduceProductService orderProduceProductService)
    {
        _context = context;
        _orderProduceProductService = orderProduceProductService;
    }

    public async Task<string> ExportExcel(List<int> ids)
    {
        var orders = await _context.OrderProduceProducts.Where(x => ids.Contains(x.Id)).ToListAsync();
        var orderDetails = await _context.OrderProduceProductDetails.Where(x => ids.Contains(x.OrderProduceProductId)).ToListAsync();
        var customerIds = orders.Select(x => x.CustomerId);
        var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();
        var goodIds = orderDetails.Select(x => x.GoodsId).Distinct();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/OrderProduceProductTemplate.xlsx");
        MemoryStream stream = new MemoryStream();
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 5;
                int nCol = 10;
                int iRow = nRowBegin;
                int index = 0;
                foreach (var order in orders)
                {
                    index++;
                    var orderDetailChecks = orderDetails.Where(x => x.OrderProduceProductId == order.Id);
                    var customer = customers.FirstOrDefault(x => x.Id == order.CustomerId);
                    worksheet.Cells[iRow, 1].Value = index.ToString();
                    worksheet.Cells[iRow, 2].Value = customer.Code;
                    worksheet.Cells[iRow, 3].Value = customer.Name;
                    worksheet.Cells[iRow, nCol].Value = order.Note;
                    int iRowBeginMerge = iRow;

                    foreach (var item in orderDetailChecks)
                    {
                        var good = goods.FirstOrDefault(x => x.Id == item.GoodsId);
                        worksheet.Cells[iRow, 4].Value = GoodNameGetter.GetCodeFromGood(good);
                        worksheet.Cells[iRow, 5].Value = GoodNameGetter.GetNameFromGood(good);
                        worksheet.Cells[iRow, 6].Value = item.QuantityRequired;
                        worksheet.Cells[iRow, 7].Value = item.UnitPrice;
                        worksheet.Cells[iRow, 8].Value = item.DiscountPrice;
                        worksheet.Cells[iRow, 9].Value = item.TaxVAT;
                        iRow++;
                    }
                    worksheet.Cells[iRowBeginMerge, 1, iRow - 1, 1].Merge = true;
                    worksheet.Cells[iRowBeginMerge, 2, iRow - 1, 2].Merge = true;
                    worksheet.Cells[iRowBeginMerge, 3, iRow - 1, 3].Merge = true;
                    worksheet.Cells[iRowBeginMerge, nCol, iRow - 1, nCol].Merge = true;
                }

                iRow--;

                if (iRow >= nRowBegin)
                {
                    worksheet.Cells[nRowBegin,6, iRow, 9].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

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
                return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "DonHang");
            }
        }
    }
    public async Task ImportExcel(IFormFile file, int userId)
    {

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets.First();
            var i = 5;
            var orders = new List<OrderProduceProductCreateModel>();

            while (sheet.Cells[i, 4].Value != null)
            {
                var order = new OrderProduceProductCreateModel();
                var customerCode = sheet.Cells[i, 2].Value.ToString();
                var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Code == customerCode);
                if (customer is null)
                {
                    throw new ErrorException("Không tìm thấy khách hàng " + customerCode);
                }
                order.CustomerId = customer.Id;
                order.Date = DateTime.Now;
                order.Note = sheet.Cells[i, 10].Value.ToString();
                order.Items = new List<OrderProduceProductDetailSetterModel>();
                {
                    var goodCode = sheet.Cells[i, 4].Value.ToString();
                    var good = await _context.Goods.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.Detail2) ? x.Detail2 == goodCode : x.Detail1 == goodCode);
                    if (good is null)
                    {
                        throw new ErrorException("Không tìm thấy hàng hóa " + goodCode);
                    }
                    var detail = new OrderProduceProductDetailSetterModel
                    {
                        GoodsId = good.Id,
                        Quantity = sheet.Cells[i, 6].Value != null ? double.Parse(sheet.Cells[i, 6].Value.ToString()) : 0,
                        UnitPrice = sheet.Cells[i, 7].Value != null ? double.Parse(sheet.Cells[i, 7].Value.ToString()) : 0,
                        DiscountPrice = sheet.Cells[i, 8].Value != null ? double.Parse(sheet.Cells[i, 8].Value.ToString()) : 0,
                        TaxVAT = sheet.Cells[i, 9].Value != null ? double.Parse(sheet.Cells[i, 9].Value.ToString()) : 0,
                    };
                    order.Items.Add(detail);
                    i++;
                }
                while (sheet.Cells[i, 2].Value == null && sheet.Cells[i, 4].Value != null)
                {
                    var goodCode = sheet.Cells[i, 4].Value.ToString();
                    var good = await _context.Goods.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.Detail2) ? x.Detail2 == goodCode : x.Detail1 == goodCode);
                    if (good is null)
                    {
                        throw new ErrorException("Không tìm thấy hàng hóa " + goodCode);
                    }
                    var detail = new OrderProduceProductDetailSetterModel
                    {
                        GoodsId = good.Id,
                        Quantity = sheet.Cells[i, 6].Value != null ? double.Parse(sheet.Cells[i, 6].Value.ToString()) : 0,
                        UnitPrice = sheet.Cells[i, 7].Value != null ? double.Parse(sheet.Cells[i, 7].Value.ToString()) : 0,
                        DiscountPrice = sheet.Cells[i, 8].Value != null ? double.Parse(sheet.Cells[i, 8].Value.ToString()) : 0,
                        TaxVAT = sheet.Cells[i, 9].Value != null ? double.Parse(sheet.Cells[i, 9].Value.ToString()) : 0,
                    };
                    order.Items.Add(detail);
                    i++;
                }
                orders.Add(order);
            }
            foreach (var order in orders)
            {
                await _orderProduceProductService.Create(order, userId);
            }
        }
    }
}
