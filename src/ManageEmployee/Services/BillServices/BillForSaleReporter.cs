using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services.BillServices;

public class BillForSaleReporter: IBillForSaleReporter
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public BillForSaleReporter(IMapper mapper, ApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }


    public async Task<PagingResult<BillDetailViewPaging>> BaoCaoLoiNhuanTruThue(int pageIndex, int pageSize, DateTime startDate,
        DateTime endDate, int type)
    {
        if (pageSize <= 0)
            pageSize = 20;

        if (pageIndex < 0)
            pageIndex = 1;
        endDate = endDate.AddDays(1);
        var listBill = await _context.Bills.Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate).ToListAsync();
        List<int> listBillId = listBill.Select(x => x.Id).ToList();
        var listBillDetail = _context.BillDetails.Where(x => listBillId.Contains(x.BillId))
            .OrderBy(x => x.CreatedDate).Select(x => _mapper.Map<BillDetailViewPaging>(x)).ToList();
        var listGood = await _context.Goods.ToListAsync();

        foreach (var item in listBillDetail)
        {
            var good = listGood.FirstOrDefault(x => x.Id == item.GoodsId);
            if (good != null)
                item.GoodsName = string.IsNullOrEmpty(good.Detail2) ? good.Detail1 : good.Detail2;
            item.Profit = item.UnitPrice + (type == 1 ? item.TaxVAT : 0) - item.Price - item.DiscountPrice;
        }

        if (pageIndex > 0)
            listBillDetail.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        return new PagingResult<BillDetailViewPaging>()
        {
            CurrentPage = pageIndex,
            PageSize = pageSize,
            TotalItems = listBillDetail.Count(),
            Data = listBillDetail
        };
    }

    public async Task<PagingResult<BillDetailReport>> BaoCaoDoanhThuTheoNgay(int pageIndex, int pageSize, DateTime startDate,
        DateTime endDate)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;
            endDate = endDate.AddDays(1);

            var listBill = await _context.Bills.Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate)
                .ToListAsync();
            List<int> listBillId = listBill.Select(x => x.Id).ToList();
            var listBillDetail = _context.BillDetails
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate).OrderBy(x => x.CreatedDate)
                .ToList();

            List<BillDetailReport> listOut = new List<BillDetailReport>();
            for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                BillDetailReport itemOut = new BillDetailReport();
                itemOut.CreatedDate = dt;
                DateTime endDateCheck = dt.AddDays(1);
                var billDetails = listBillDetail.Where(x => x.CreatedDate >= dt && x.CreatedDate < endDateCheck)
                    .ToList();
                if (billDetails.Count == 0)
                    continue;
                itemOut.Price = billDetails.Sum(x => x.UnitPrice * x.Quantity);
                itemOut.TaxVAT = billDetails.Sum(x => x.TaxVAT * x.Quantity);
                itemOut.DiscountPrice = billDetails.Sum(x => x.DiscountPrice * x.Quantity);
                var billTMIds = listBill
                    .Where(x => x.CreatedDate >= dt && x.CreatedDate < endDateCheck && x.TypePay == "TM")
                    .Select(x => x.Id).ToList();
                var billCNIds = listBill
                    .Where(x => x.CreatedDate >= dt && x.CreatedDate < endDateCheck && x.TypePay == "CN")
                    .Select(x => x.Id).ToList();
                var billNHIds = listBill
                    .Where(x => x.CreatedDate >= dt && x.CreatedDate < endDateCheck && x.TypePay == "NH")
                    .Select(x => x.Id).ToList();

                var billDetailTMs = billDetails.Where(x => billTMIds.Contains(x.BillId))
                    .Sum(x => x.Quantity * (x.UnitPrice + x.TaxVAT));
                var billDetailCNs = billDetails.Where(x => billCNIds.Contains(x.BillId))
                    .Sum(x => x.Quantity * (x.UnitPrice + x.TaxVAT));
                var billDetailNHs = billDetails.Where(x => billNHIds.Contains(x.BillId))
                    .Sum(x => x.Quantity * (x.UnitPrice + x.TaxVAT));

                itemOut.TienMat = billDetailTMs;
                itemOut.CongNo = billDetailCNs;
                itemOut.NganHang = billDetailNHs;
                itemOut.PhaiThu = itemOut.TienMat + itemOut.CongNo + itemOut.NganHang;

                listOut.Add(itemOut);
            }

            {
                BillDetailReport itemOut = new BillDetailReport();
                itemOut.CreatedDate = null;

                itemOut.Price = listBillDetail.Sum(x => x.UnitPrice * x.Quantity);
                itemOut.TaxVAT = listBillDetail.Sum(x => x.TaxVAT * x.Quantity);
                itemOut.DiscountPrice = listBillDetail.Sum(x => x.DiscountPrice * x.Quantity);
                var billTMIds = listBill.Where(x => x.TypePay == "TM").Select(x => x.Id).ToList();
                var billCNIds = listBill.Where(x => x.TypePay == "CN").Select(x => x.Id).ToList();
                var billNHIds = listBill.Where(x => x.TypePay == "NH").Select(x => x.Id).ToList();

                var billDetailTMs = listBillDetail.Where(x => billTMIds.Contains(x.BillId))
                    .Sum(x => x.Quantity * (x.UnitPrice + x.TaxVAT));
                var billDetailCNs = listBillDetail.Where(x => billCNIds.Contains(x.BillId))
                    .Sum(x => x.Quantity * (x.UnitPrice + x.TaxVAT));
                var billDetailNHs = listBillDetail.Where(x => billNHIds.Contains(x.BillId))
                    .Sum(x => x.Quantity * (x.UnitPrice + x.TaxVAT));


                itemOut.TienMat = billDetailTMs;
                itemOut.CongNo = billDetailCNs;
                itemOut.NganHang = billDetailNHs;
                itemOut.PhaiThu = itemOut.TienMat + itemOut.CongNo + itemOut.NganHang;

                listOut.Add(itemOut);
            }

            if (pageIndex > 0)
                listOut.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PagingResult<BillDetailReport>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = listOut.Count,
                Data = listOut
            };
        }
        catch
        {
            return new PagingResult<BillDetailReport>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = null
            };
        }
    }

    public async Task<string> BaoCaoDoanhThuTheoNgayExcel(SearchViewModel param)
    {
        string _fileMapServer = $"BaoCaoDoanhThuTheoNgay_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
               folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
               _pathSave = Path.Combine(folder, _fileMapServer);

        DateTime startDate = param.StartDate ?? DateTime.Today;
        DateTime endDate = param.EndDate ?? DateTime.Today;
        List<BillDetailReport> listBillDetail = (await BaoCaoDoanhThuTheoNgay(param.Page, param.PageSize, startDate, endDate)).Data;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\BAO-CAO-THEO-NGAY.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 5;
                int rowIdx = nRowBegin + 1;
                if (listBillDetail.Count > 0)
                {
                    foreach (var item in listBillDetail)
                    {
                        if (item.CreatedDate == null)
                        {
                            sheet.Cells[nRowBegin, 1].Value = "TỔNG CỘNG";
                            sheet.Cells[nRowBegin, 2].Value = item.Price;
                            sheet.Cells[nRowBegin, 3].Value = item.TaxVAT;
                            sheet.Cells[nRowBegin, 4].Value = item.DiscountPrice;

                            sheet.Cells[nRowBegin, 5].Value = item.PhaiThu;
                            sheet.Cells[nRowBegin, 6].Value = item.TienMat;
                            sheet.Cells[nRowBegin, 7].Value = item.NganHang;
                            sheet.Cells[nRowBegin, 8].Value = item.CongNo;
                        }
                        else
                        {
                            sheet.Cells[rowIdx, 1].Value = (item.CreatedDate ?? DateTime.Today).ToString("dd/MM/yyyy");
                            sheet.Cells[rowIdx, 2].Value = item.Price;
                            sheet.Cells[rowIdx, 3].Value = item.TaxVAT;
                            sheet.Cells[rowIdx, 4].Value = item.DiscountPrice;

                            sheet.Cells[rowIdx, 5].Value = item.PhaiThu;
                            sheet.Cells[rowIdx, 6].Value = item.TienMat;
                            sheet.Cells[rowIdx, 7].Value = item.NganHang;
                            sheet.Cells[rowIdx, 8].Value = item.CongNo;
                            rowIdx++;
                        }
                    }
                }

                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 2, rowIdx, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 8].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }
            }
        }

        return _fileMapServer;
    }

    public async Task<string> BaoCaoLoiNhuanTruThueExcel(SearchViewModel param)
    {
        string _fileMapServer = $"DanhSachTaiKhoan_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
               folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
               _pathSave = Path.Combine(folder, _fileMapServer);

        List<BillDetailViewPaging> listBillDetail = (await BaoCaoLoiNhuanTruThue(param.Page, param.PageSize, param.StartDate ?? DateTime.Today, param.EndDate ?? DateTime.Today, param.Type ?? 1)).Data;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\BAO-CAO-LOI-NHUAN-TAM-TINH-TRU-THUE.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 5;
                int rowIdx = nRowBegin + 1;
                if (listBillDetail.Count > 0)
                {
                    double sumPrice = 0;
                    double sumUnitPrice = 0;
                    double sumDiscountPrice = 0;
                    foreach (var item in listBillDetail)
                    {
                        sheet.Cells[rowIdx, 1].Value = item.CreatedDate.ToString("dd/MM/yyyy");
                        sheet.Cells[rowIdx, 2].Value = item.GoodsName;
                        sheet.Cells[rowIdx, 3].Value = item.Price;
                        sheet.Cells[rowIdx, 4].Value = item.UnitPrice;
                        sheet.Cells[rowIdx, 5].Value = item.DiscountPrice;
                        sheet.Cells[rowIdx, 6].Value = item.Profit;
                        sheet.Cells[rowIdx, 7].Value = "";
                        rowIdx++;
                        sumPrice += item.Price;
                        sumUnitPrice += item.UnitPrice;
                        sumDiscountPrice += item.DiscountPrice;
                    }
                    {
                        sheet.Cells[nRowBegin, 1, nRowBegin, 2].Merge = true;
                        sheet.Cells[nRowBegin, 1].Value = "TỔNG CỘNG";
                        sheet.Cells[nRowBegin, 3].Value = sumPrice;
                        sheet.Cells[nRowBegin, 4].Value = sumUnitPrice;
                        sheet.Cells[nRowBegin, 5].Value = sumDiscountPrice;

                        sheet.Cells[nRowBegin, 6].Value = sumUnitPrice - sumPrice - sumDiscountPrice;
                        sheet.Cells[nRowBegin, 7].Value = "";
                    }
                }
                rowIdx--;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 3, rowIdx, 6].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 7].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }
            }
        }
        return _fileMapServer;
    }

}
