using Common.Constants;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Reports;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.DataTransferObject.SelectModels;

namespace ManageEmployee.Services.Reports;


public class VoucherService : IVoucherService
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter converterPDF;
    public VoucherService(ApplicationDbContext context, IConverter _converPDF)
    {
        _context = context;
        converterPDF = _converPDF;
    }

    /// <summary>
    ///  Lấy chứng từ ghi sổ
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="voucherType"></param>
    /// <returns></returns>
    public async Task<List<VoucherReportItem>> GenerateVoucherReportV2(DateTime dtfrom, DateTime dtto, string InvoiceTaxCode, string InvoiceNumber, string voucherType = "PT", bool isNoiBo = false)
    {
        // Năm định khoản
        var year = dtfrom.Year;
        var type = string.IsNullOrEmpty(voucherType) ? "" : voucherType.ToUpper();
        List<VoucherReportItem> results = new List<VoucherReportItem>();

        var ledgerDatas = await _context.GetLedger(year, isNoiBo ? 3 : 2)
         .Where(x => string.IsNullOrEmpty(voucherType) || x.Type == type)
         .Where(x => x.OrginalBookDate.Value >= dtfrom && x.OrginalBookDate.Value <= dtto)
         .Where(x => string.IsNullOrEmpty(InvoiceTaxCode) || x.InvoiceTaxCode == InvoiceTaxCode)
         .Where(x => string.IsNullOrEmpty(InvoiceNumber) || x.InvoiceNumber == InvoiceNumber)
         .ToListAsync();
        List<IGrouping<int, Ledger>>  voucherLists = ledgerDatas.GroupBy(x => x.Month).ToList();

        for (int i = 0; i < voucherLists.Count; i++)
	        {
		        var voucherReportItem = new VoucherReportItem();
		        voucherReportItem.VoucherNumber = i.ToString();
		        voucherReportItem.Date = new DateTime(year, voucherLists[i].Key, DateTime.DaysInMonth(year, voucherLists[i].Key));
		        var count = voucherLists[i].GroupBy(x => x.OrginalVoucherNumber).Count();
		        voucherReportItem.IncludedVoucherCount = string.Format("Kèm theo {0} chứng từ", count);
		        voucherReportItem.Title = String.Format("Tổng hợp định khoản các chứng từ gốc thuộc chứng từ ghi sổ - Số: {0}", i);
		        // Nhóm 2 cặp nợ có
		        var group = voucherLists[i].GroupBy(x => new { DebitCode = x.DebitCode.Substring(0, 3), CreditCode = x.CreditCode.Substring(0, 3) })
		        .OrderBy(x => x.Key.DebitCode).ToList();

		        voucherReportItem.Details.Debit =
		        group
		        .Select(x => new VoucherAccount()
		        {
			        Code = x.Key.DebitCode,
			        Amount = x.Sum(c => c.Amount)
		        })
		        .ToList();

		        // Tổng hợp lại
		        if (voucherReportItem.Details.Debit.GroupBy(x => x.Code).Count() == 1) {
			        voucherReportItem.Details.Debit = voucherReportItem.Details.Debit.GroupBy(x => x.Code)
			        .Select(x => new VoucherAccount()
			        {
				        Code = x.Key,
				        Amount = x.Sum(c => c.Amount)
			        })
			        .ToList();
		        }

		        voucherReportItem.Details.Credit =
		        group
		        .Select(x => new VoucherAccount()
		        {
			        Code = x.Key.CreditCode,
			        Amount = x.Sum(c => c.Amount)
		        })
		        .ToList();

		        // Tổng hợp lại
		        if (voucherReportItem.Details.Credit.GroupBy(x => x.Code).Count() == 1)
		        {
			        voucherReportItem.Details.Credit = voucherReportItem.Details.Credit.GroupBy(x => x.Code)
			        .Select(x => new VoucherAccount()
			        {
				        Code = x.Key,
				        Amount = x.Sum(c => c.Amount)
			        })
			        .ToList();
		        }

		        results.Add(voucherReportItem);
	        }

        return results;

    }
    /// <summary>
    /// Lấy bảng kê chứng từ ghi sổ
    /// </summary>
    /// <param name="year"></param>
    /// <param name="voucherType"></param>
    /// <returns></returns>
    public async Task<List<VoucherInforItem>> GenerateTransactionList(DateTime from, DateTime to,
        int year, string voucherType = "PT", bool isNoiBo = false, string InvoiceNumber = "", string InvoiceTaxCode = "")
    {
        var reportItems = await _context.GetLedger(year, isNoiBo ? 3 : 2)
            .Where(x => x.Type == voucherType
                            && x.OrginalBookDate.Value >= from
                            && x.OrginalBookDate.Value < to
                            && (string.IsNullOrEmpty(InvoiceNumber) || x.InvoiceNumber == InvoiceNumber)
                            && (string.IsNullOrEmpty(InvoiceTaxCode) || x.InvoiceTaxCode == InvoiceTaxCode)
                            )
            .OrderByDescending(y => y.OrginalBookDate)
        .ToListAsync();

        return reportItems
        .GroupBy(x => x.Month).Select(x => new VoucherInforItem
        {
            VoucherNumber = x.First().VoucherNumber,
            // Kết sổ
            ClosedDate = x.FirstOrDefault().OrginalBookDate ?? DateTime.Now,// Cuối tháng
            Vouchers = x.Select(y => new VoucherDetail
            {
                BookDate = y.BookDate,
                OrginalVoucherNumber = y.OrginalVoucherNumber,
                OrginalBookDate = y.OrginalBookDate.Value,
                VoucherDescription = y.OrginalDescription,
                DebitCode = y.DebitCode,
                CreditCode = y.CreditCode + " " + y.CreditDetailCodeFirst,
                Quantity = y.Quantity,
                Amount = y.Amount,
                RefVoucher = y.AttachVoucher,
                RefInvoice = y.InvoiceNumber,
                Month = y.Month
            }).ToList()
        }).ToList();
    }

    public string ExportDataVoucher(VoucherReportViewModel voucher, VoucherReportParam param)
    {
        try
        {

            var documents = _context.Documents.Where(x => x.Code == voucher.Type).FirstOrDefault();
            if (documents != null)
            {
                voucher.Type = documents.Code;
                voucher.TypeName = documents.Name;
            }

            string _path = string.Empty;
            switch (param.FileType)
            {
                case "html":
                    _path = ConvertToHTML_Final(voucher, param.FromMonth, param.ToMonth, param.FromDate, param.ToDate, param.VoucherType, param.VoteMaker, param.isCheckName);
                    break;
                case "excel":
                    _path = ExportExcel_Report(voucher, param.VoteMaker, param.isCheckName);
                    break;
                case "pdf":
                    _path = ConvertToPDFFile(voucher, param.FromMonth, param.ToMonth, param.FromDate, param.ToDate, param.VoucherType, param.VoteMaker, param.isCheckName);
                    break;
            }
            return _path;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report(VoucherReportViewModel p, string preparedBy, bool isFillName)
    {
        try
        {

            if (p == null) return string.Empty;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetName");

            //A => J
            worksheet.Cells["A1:I1"].Value = p.Company;
            worksheet.Cells["A2:I2"].Value = p.Address;
            worksheet.Cells["A3:I3"].Value = p.TaxId;

            worksheet.Cells["A1:I1"].Merge = true;
            worksheet.Cells["A2:I2"].Merge = true;
            worksheet.Cells["A3:I3"].Merge = true;
            worksheet.Cells["A4:I4"].Merge = true;

            worksheet.Cells["D5:F5"].Merge = true;
            worksheet.Cells["D5:F5"].Value = "CHỨNG TỪ GHI SỔ";

            worksheet.Cells["H5"].Value = "Số: ";
            worksheet.Cells["I5"].Value = p.Items.OrderByDescending(x => x.Date).FirstOrDefault()?.VoucherNumber;

            worksheet.Cells["D6:F6"].Merge = true;
            worksheet.Cells["D6:F6"].Value = "Loại chứng từ: " + p.Type + " - " + p.TypeName;

            worksheet.Cells["H6"].Value = "Ngày: ";
            worksheet.Cells["I6"].Value = p.Items.OrderByDescending(x => x.Date).FirstOrDefault()?.Date.ToString("dd/MM/yyyy");



            //table
            worksheet.Cells["A8:C9"].Merge = true;
            worksheet.Cells["A8:C9"].Value = "TRÍCH YẾU";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["A8:C9"]);

            worksheet.Cells["D8:F8"].Merge = true;
            worksheet.Cells["D8:F8"].Value = "GHI NỢ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["D8:F8"]);

            worksheet.Cells["G8:I8"].Merge = true;
            worksheet.Cells["G8:I8"].Value = "GHI CÓ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G8:I8"]);

            worksheet.Cells["D9"].Value = "TK"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["D9"]);

            worksheet.Cells["E9:F9"].Merge = true;
            worksheet.Cells["E9:F9"].Value = "SỐ TIỀN"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E9:F9"]);


            worksheet.Cells["G9"].Value = "TK"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G9"]);

            worksheet.Cells["H9:I9"].Merge = true;
            worksheet.Cells["H9:I9"].Value = "SỐ TIỀN"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["H9:I9"]);

            int currentRowNo = 9, flagRowNo = 0;
            double sumDebit = 0, sumCredit = 0;

            currentRowNo++;
            for (int i = 0; i < p.Items.Count; i++)
            {
                VoucherReportItem dr = p.Items[i];
                VoucherSubject _subject = dr.Details;
                flagRowNo = currentRowNo;
                int _maxRowLoop = _subject.Debit.Count > _subject.Credit.Count ? _subject.Debit.Count : _subject.Credit.Count;

                //chỉ show 1 dòng debit vs số tiền tổng
                if (_subject.Debit.Count > 0)
                {
                    _subject.Debit = new List<VoucherAccount> { _subject.Debit[0] };
                    _subject.Debit[0].Amount = dr.DebitTotalAmount;
                }

                for (int jR = 0; jR < _maxRowLoop; jR++)
                {
                    worksheet.Cells[currentRowNo, 8, currentRowNo, 9].Merge = true;


                    //chỉ show 1 dòng debit
                    if (_subject.Debit.Count > jR)
                    {
                        VoucherAccount _vaDebit = _subject.Debit[jR];
                        worksheet.Cells[currentRowNo, 4, currentRowNo + _subject.Credit.Count - 1, 4].Merge = true;
                        worksheet.Cells[currentRowNo, 4, currentRowNo + _subject.Credit.Count - 1, 4].Value = _vaDebit.Code;
                        worksheet.Cells[currentRowNo, 5, currentRowNo + _subject.Credit.Count - 1, 6].Value = _vaDebit.Amount;
                        worksheet.Cells[currentRowNo, 5, currentRowNo + _subject.Credit.Count - 1, 6].Merge = true;

                    }


                    if (_subject.Credit.Count > jR)
                    {
                        VoucherAccount _va = _subject.Credit[jR];
                        worksheet.Cells[currentRowNo, 7].Value = _va.Code;
                        worksheet.Cells[currentRowNo, 8, currentRowNo, 9].Value = _va.Amount;
                    }

                    if (_subject.Debit.Count > 0 || _subject.Credit.Count > 0)
                        currentRowNo++;
                }

                worksheet.Cells[flagRowNo, 1, currentRowNo - 1, 3].Merge = true;
                worksheet.Cells[flagRowNo, 1, currentRowNo - 1, 3].Value = dr.Title + " " + dr.VoucherNumber;
            }

            sumDebit = p.Items.Sum(x => x.Details.TotalDebitAmount);
            sumCredit = p.Items.Sum(x => x.Details.TotalCreditAmount);

            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "CỘNG";
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Value = sumDebit;
            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNo, 8, currentRowNo, 9].Merge = true;
            worksheet.Cells[currentRowNo, 8, currentRowNo, 9].Value = sumCredit;
            worksheet.Cells[currentRowNo, 8, currentRowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNo, 1, currentRowNo, 9].Style.Font.Bold = true;

            worksheet.Cells[10, 5, currentRowNo, 5].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";
            worksheet.Cells[10, 8, currentRowNo, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Kèm theo 0 chứng từ";

            currentRowNo += 2;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Value = "Ngày ..... tháng ..... năm ..... ";
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            currentRowNo++;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Value = "Kế toán trưởng";
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Người lập phiếu";
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = (!string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty);

            if (isFillName)
            {
                worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Merge = true;
                worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Value = p.ChiefAccountantName;
            }

            worksheet.Column(1).AutoFit(25);
            worksheet.Column(2).AutoFit(30);
            worksheet.Column(3).AutoFit(15);
            worksheet.Column(4).AutoFit(15);
            worksheet.Column(5).AutoFit(15);
            worksheet.Column(6).AutoFit(15);
            worksheet.Column(7).AutoFit(15);
            worksheet.Column(8).AutoFit(15);
            worksheet.Column(9).AutoFit(15);

            worksheet.SelectedRange["A1:I3"].Style.Font.Size = 12;
            worksheet.SelectedRange["A6:I6"].Style.Font.Size = 12;
            worksheet.SelectedRange["H5"].Style.Font.Size = 12;

            worksheet.SelectedRange["A5:G5"].Style.Font.Size = 16;
            worksheet.SelectedRange["A5:G5"].Style.Font.Bold = true;
            worksheet.Row(5).Height = 30;

            worksheet.SelectedRange["A8:I9"].Style.Font.Bold = true;
            worksheet.SelectedRange["A8:I9"].Style.Font.Size = 14;

            worksheet.Cells["A8:I9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.SelectedRange["A8:I9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 213, 180));
            worksheet.SelectedRange["A10:I" + (currentRowNo - 6)].Style.Font.Bold = false;
            worksheet.SelectedRange["A10:I" + currentRowNo].Style.Font.Size = 12;

            worksheet.SelectedRange["A1:I" + currentRowNo].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.SelectedRange["A3:I" + currentRowNo].Style.WrapText = true;

            worksheet.SelectedRange["A5:I10"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRowNo - 3, 1, currentRowNo + 3, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRowNo - 3, 1, currentRowNo + 3, 9].Style.Font.Bold = true;

            worksheet.SelectedRange["A1:B3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            worksheet.SelectedRange["A10:C" + (currentRowNo - 6)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            worksheet.SelectedRange["D10:D" + (currentRowNo - 6)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            worksheet.SelectedRange["G10:G" + (currentRowNo - 6)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            worksheet.SelectedRange["E10:F" + (currentRowNo - 5)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            worksheet.SelectedRange["H10:I" + (currentRowNo - 5)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

            return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "ChungTuGhiSo");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToPDFFile(VoucherReportViewModel p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            string _allText = ConvertToHTML_Final(p, fromMonth, fromMonth, fromDate, toDate, voucherType, preparedBy, isFillName);
            return ExcelHelpers.ConvertUseDink(_allText, converterPDF, Directory.GetCurrentDirectory(), "ChungTuGhiSo");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToHTML_Final(VoucherReportViewModel p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            if (p != null)
            {
                if (p.Items.Count > 0)
                {
                    string resultHTML_Total = string.Empty;
                    p.Items.ForEach(x =>
                    {
                        VoucherReportViewModel _eachRPT = p;
                        x.Subjects = new List<VoucherSubject> { x.Details };
                        _eachRPT.Items = new List<VoucherReportItem> { x };
                        string _eachHTML = ConvertToHTML_V2(_eachRPT, fromMonth, toMonth, fromDate, toDate, preparedBy, isFillName);
                        resultHTML_Total += _eachHTML;
                    });

                    return AttachHTMLToSrc(resultHTML_Total);
                }
                else
                {
                    var documents = _context.Documents.FirstOrDefault(x => x.Code == voucherType);

                    VoucherReportViewModel _eachRPT = new()
                    {
                        Type = voucherType ,
                        TypeName = documents?.Name
                    };
                    string _eachHTML = ConvertToHTML_V2(_eachRPT, fromMonth, toMonth, fromDate, toDate, preparedBy, isFillName);
                    return AttachHTMLToSrc(_eachHTML);

                }
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToHTML_V2(VoucherReportViewModel p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string preparedBy, bool isFillName)
    {
        try
        {
            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;

            string _template = "ChungTuEachTemplate.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "LoaiChungTu", p.Type+" - "+ p.TypeName },
                { "SoCT", p.Items.Count > 0 ? p.Items.OrderByDescending(x => x.Date).First().VoucherNumber : string.Empty },
                { "MaxNgay", p.Items.Count > 0 ? p.Items.OrderByDescending(x => x.Date).First().Date.ToString("dd/MM/yyyy") : string.Empty },
                { "TuThang", ((fromMonth > 0 && toMonth > 0) ? ((int)fromMonth) : ((DateTime)fromDate).Month ).ToString("D2")   },
                { "DenThang", ( (fromMonth > 0 && toMonth > 0) ? ((int)toMonth) : ((DateTime)toDate).Month ).ToString("D2") },
                { "Nam", ((fromMonth > 0 && toMonth > 0) ? DateTime.Now.Year : ((DateTime)fromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty },
                { "KeToanTruong", isFillName ? p.ChiefAccountantName : string.Empty },
                { "KeToanTruong_CV", p.NoteChiefAccountantName },
                { "Ngay", "......" },
                { "Thang", "......" },
                { "TONG_TK_NO", string.Empty },
                { "TONG_SOTIEN_NO", string.Format("{0:N0}", p.Items.Sum(x => x.Subjects.Sum(y => y.TotalDebitAmount) ) ) },
                { "TONG_TK_CO", string.Empty },
                { "TONG_SOTIEN_CO", string.Format("{0:N0}", p.Items.Sum(x => x.Subjects.Sum(y => y.TotalCreditAmount)  ) ) },
                { "CountSoCT", p.Items != null ? p.Items.Count > 0 ? p.Items[0].IncludedVoucherCount : string.Empty : string.Empty }
            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            if (p.Items.Count > 0)
            {
                int _count = 0;
                p.Items.ForEach(x =>
                {
                    _count++;
                    x.Subjects.ForEach(y =>
                    {
                        //chỉ show 1 dòng debit vs tổng tiền debit
                        int _maxIndexRow = y.Debit.Count > y.Credit.Count ? y.Debit.Count : y.Credit.Count;
                        string _resultCredit = string.Empty, _resultDebit = string.Empty,
                        _firstCredite = string.Empty, _firstDebit = string.Empty,
                        _rowSpecific = string.Empty;

                        string _debitHTML_2 = "<td class='nemo-td'>{{{TK_NO}}}</td><td class='txt-right nemo-td'>{{{SOTIEN_NO}}}</td>",
                         _creditHTML_2 = "<td class='nemo-td'>{{{TK_CO}}}</td><td class='txt-right nemo-td'>{{{SOTIEN_CO}}}</td>";


                        for (int i = 0; i < _maxIndexRow; i++)
                        {
                            string _debitHTML_Inside = string.Empty,
                             _creditHTML_Inside = string.Empty;

                            if (i < y.Debit.Count)
                            {
                                VoucherAccount voucherCredit = y.Debit[i];
                                _debitHTML_Inside =
                                _debitHTML_2.Replace("{{{TK_NO}}}", voucherCredit.Code)
                                .Replace("{{{SOTIEN_NO}}}", String.Format("{0:N0}", voucherCredit.Amount));
                            }
                            else
                                _debitHTML_Inside = "<td class='nemo-td'></td><td class='nemo-td'></td>";

                            if (i < y.Credit.Count)
                            {
                                VoucherAccount voucherCredit = y.Credit[i];
                                _creditHTML_Inside =
                                _creditHTML_2.Replace("{{{TK_CO}}}", voucherCredit.Code)
                                .Replace("{{{SOTIEN_CO}}}", String.Format("{0:N0}", voucherCredit.Amount));
                            }
                            else
                                _creditHTML_Inside = "<td class='nemo-td'></td><td class='nemo-td'></td>";


                            _resultDebit = _debitHTML_Inside;
                            _resultCredit = _creditHTML_Inside;

                            if (i > 0)
                                _rowSpecific += $"<tr class='chung-tu-col-nm'>{_resultDebit}{_resultCredit}</tr>";
                            else
                            {
                                _firstDebit = _resultDebit;
                                _firstCredite = _resultCredit;
                            }

                        }



                        string _resultRow = @"<tr class='chung-tu-col-nm'>
                                                    <td rowspan='#ROW_SPAN#' colspan='2'>{{{TRICH_YEU}}}</td>
                                                    #CREDIT_ONE_ROW#
                                                </tr>";

                        _resultRow = _resultRow.Replace("{{{TRICH_YEU}}}", x.Title + " " + x.VoucherNumber)
                                    .Replace("#CREDIT_ONE_ROW#", _firstDebit + _firstCredite)
                                    .Replace("#ROW_SPAN#", _maxIndexRow.ToString())
                                    ;
                        _resultRow += _rowSpecific;

                        resultStr += _resultRow;

                    });

                });

            }
            _allText = _allText
                .Replace("##REPLACE_PLACE##", resultStr)
                .Replace("#ROW_SPAN#", "0");

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string AttachHTMLToSrc(string _html)
    {
        string _template = "ChungTuTotalTemplate.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
        return _allText.Replace("##REPLACE_ALL_TABLE##", _html);
    }

    #region FOR TRANSACTION LIST REPORT
    public string ExportDataTransactionList(TransactionListViewModel voucher, TransactionListParam param)
    {
        try
        {
            string _path = string.Empty;

            var _doc = _context.Documents.Where(x => x.Code == voucher.Type).FirstOrDefault();
            if(_doc != null)
            {
                voucher.Type = _doc.Code;
                voucher.TypeName = _doc.Name;
            }

            switch (param.FileType)
            {
                case "html":
                    _path = ConvertToHTML_TransactionList_V2(voucher, param);
                    break;
                case "excel":
                    _path = ExportExcel_Report_TransactionList(voucher, param);
                    break;
                case "pdf":
                    _path = ConvertToPDFFile_TransactionList(voucher, param);
                    break;
            }
            return _path;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToHTML_TransactionList_V2(TransactionListViewModel p, TransactionListParam param)
    {
        try
        {
            if (p == null) return string.Empty;
            if (param.FromMonth == null) param.FromMonth = 0;
            if (param.ToMonth == null) param.ToMonth = 0;
            if (param.FromDate == null) param.FromDate = DateTime.Now;
            if (param.ToDate == null) param.ToDate = DateTime.Now;

            string _htmlTotal = string.Empty;
            p.VoucherInfors.ForEach(x =>
            {
                TransactionListViewModel _k = p;
                _k.VoucherInfors = new List<VoucherInforItem> { x };

                string _htmlEach = GetEachHTML_Transaction(_k, param);
                _htmlTotal += _htmlEach;
            });

            if(p.VoucherInfors.Count == 0)
            {
                TransactionListViewModel _k = new();
                string _htmlEach = GetEachHTML_Transaction(_k, param);
                _htmlTotal += _htmlEach;
            }

            string _template = "BangKeChungTuTotalTemplate.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
            _allText = _allText.Replace("##TOTAL_REPLACE_PLACE##", _htmlTotal);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string GetEachHTML_Transaction(TransactionListViewModel p, TransactionListParam param)
    {
        try
        {
            if (p == null) return string.Empty;
            if (param.FromMonth == null) param.FromMonth = 0;
            if (param.ToMonth == null) param.ToMonth = 0;
            if (param.FromDate == null) param.FromDate = DateTime.Now;
            if (param.ToDate == null) param.ToDate = DateTime.Now;

            string _template = "BangKeChungTuEachTemplate.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "LoaiChungTu", p.Type+" - "+ p.TypeName },
                { "NgayChungTu", p.VoucherInfors.OrderByDescending(x => x.ClosedDate).FirstOrDefault()?.ClosedDate.ToString("dd/MM/yyyy") },
                { "TuThang", ((param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.FromMonth) : ((DateTime)param.FromDate).Month ).ToString("D2")   },
                { "DenThang", ( (param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.ToMonth) : ((DateTime)param.ToDate).Month ).ToString("D2") },
                { "Nam", ((param.FromMonth > 0 && param.ToMonth > 0) ? DateTime.Now.Year : ((DateTime)param.FromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.VoteMaker) ? param.VoteMaker : string.Empty },
                { "Ngay", " ..... " },
                { "Thang", " ..... " },
                { "SoCT", p.VoucherInfors.OrderByDescending(x => x.ClosedDate).FirstOrDefault()?.VoucherNumber },
                { "KeToanTruong", p.isFillName ? p.ChiefAccountantName : string.Empty },
                { "KeToanTruong_CV", !string.IsNullOrEmpty(p.ChiefAccountantNote) ? p.ChiefAccountantNote : string.Empty },
                { "KEM_THEO_SO_CT", p == null ? "0" : p.VoucherInfors.Count == 0 ? "0" : p.VoucherInfors[0].Vouchers.Count.ToString() },
            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            int _stt = 0;
            if (p.VoucherInfors.Count > 0)
            {
                p.VoucherInfors.ForEach(x =>
                {

                    if (x.Vouchers.Count > 0)
                    {
                        x.Vouchers.ForEach(y =>
                        {
                            _stt++;
                            string _tr = @"<tr>
                                        <td colspan='2'>{{{STT}}}</td>
                                    <td>{{{NGAY_GHI_SO}}}</td>
                                    <td><a href='{{{URL_LINK}}}#{{{FILTER_TYPE}}}#{{{FILTER_TEXT}}}#{{{FILTER_MONTH}}}#{{{FILTER_ISINTERNAL}}}' target='_blank'>{{{CHUNG_TU_SO}}}</a></td>
                                    <td>{{{CHUNG_TU_NGAY}}}</td>
                                    <td>{{{DIEN_GIAI}}}</td>
                                    <td>{{{TK_NO}}}</td>
                                    <td>{{{TK_CO}}}</td>
                                    <td class='txt-right'>{{{SO_LUONG}}}</td>
                                    <td class='txt-right'>{{{SO_TIEN}}}</td>
                                    <td>{{{HOA_DON}}}</td>
                                    <td>{{{PHIEU_NHAP}}}</td>
                                    </tr>";

                            _tr = _tr
                                       .Replace("{{{STT}}}", _stt.ToString())
                                       .Replace("{{{URL_LINK}}}", UrlLinkConst.UrlLinkArise)
                                       .Replace("{{{NGAY_GHI_SO}}}", y.BookDate.Value.ToString("dd/MM"))
                                       .Replace("{{{CHUNG_TU_SO}}}", y.OrginalVoucherNumber)
                                       .Replace("{{{CHUNG_TU_NGAY}}}", y.OrginalBookDate.ToString("dd/MM/yyyy"))
                                       .Replace("{{{DIEN_GIAI}}}", y.VoucherDescription)
                                   .Replace("{{{TK_NO}}}", y.DebitCode)
                                   .Replace("{{{TK_CO}}}", y.CreditCode)
                                   .Replace("{{{SO_LUONG}}}", String.Format("{0:N0}", y.Quantity))
                                   .Replace("{{{SO_TIEN}}}", String.Format("{0:N0}", y.Amount))
                                   .Replace("{{{HOA_DON}}}", y.RefInvoice)
                                   .Replace("{{{PHIEU_NHAP}}}", y.RefVoucher)
                                                                          .Replace("{{{FILTER_TEXT}}}", y.DebitCode)
                                   .Replace("{{{FILTER_TYPE}}}", param.VoucherType)
                                   .Replace("{{{FILTER_MONTH}}}", y.Month.ToString())
                                                                          .Replace("{{{FILTER_ISINTERNAL}}}", (param.IsNoiBo ? "3" : "1"))

                                   ;

                            resultHTML += _tr;
                            
                        });

                        string _tr_Sum = @"<tr class='font-b'>
                                        <td colspan='2'></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td>Tổng cộng</td>
                                    <td></td>
                                    <td></td>
                                    <td class='txt-right'></td>
                                    <td class='txt-right'>{{{SO_TIEN}}}</td>
                                    <td></td>
                                    <td></td>
                                    </tr>";

                        _tr_Sum = _tr_Sum.Replace("{{{SO_TIEN}}}",
                            String.Format("{0:N0}", x.Vouchers.Sum(k => k.Amount))
                            );
                        resultHTML += _tr_Sum;


                    }


                });
            }

            _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }


    private string ConvertToPDFFile_TransactionList(TransactionListViewModel p, TransactionListParam param)
    {
        try
        {
            string _allText = ConvertToHTML_TransactionList_V2(p, param);
            return ExcelHelpers.ConvertUseDinkLandscape(_allText, converterPDF, Directory.GetCurrentDirectory(), "BangKeChungTuGhiSo");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report_TransactionList(TransactionListViewModel p, TransactionListParam param)
    {
        try
        {
            if (p == null) return string.Empty;
            if (param.FromMonth == null) param.FromMonth = 0;
            if (param.ToMonth == null) param.ToMonth = 0;
            if (param.FromDate == null) param.FromDate = DateTime.Now;
            if (param.ToDate == null) param.ToDate = DateTime.Now;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetName");

            //A => J
            worksheet.Cells["A1:K1"].Value = p.Company;
            worksheet.Cells["A2:K2"].Value = p.Address;
            worksheet.Cells["A3:K3"].Value = p.TaxId;
            worksheet.Cells["A1:K1"].Merge = true;
            worksheet.Cells["A2:K2"].Merge = true;
            worksheet.Cells["A3:K3"].Merge = true;

            worksheet.Cells["A4:K4"].Merge = true;

            worksheet.Cells["C5:I5"].Merge = true;
            worksheet.Cells["C5:I5"].Value = "BẢNG KÊ CHỨNG TỪ GHI SỔ";

            worksheet.Cells["J5"].Value = "Số CTGS:";
            worksheet.Cells["K5"].Value = p.VoucherInfors.OrderByDescending(x => x.ClosedDate).FirstOrDefault()?.VoucherNumber;

            worksheet.Cells["C6:I6"].Merge = true;
            worksheet.Cells["C6:I6"].Value = "Loại chứng từ: " + p.Type+" - "+p.TypeName;

            worksheet.Cells["J6"].Value = "Ngày:";
            worksheet.Cells["K6"].Value = p.VoucherInfors.OrderByDescending(x => x.ClosedDate).FirstOrDefault()?.ClosedDate.ToString("dd/MM/yyyy");

            worksheet.Cells["J7:K7"].Merge = true;
            worksheet.Cells["J7:K7"].Value = "Đơn vị tính: Đồng";

            //table
            worksheet.Cells["A8:A9"].Merge = true;
            worksheet.Cells["A8:A9"].Value = "STT";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["A8:A9"]);

            worksheet.Cells["B8:B9"].Merge = true;
            worksheet.Cells["B8:B9"].Value = "NGÀY GHI SỐ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["B8:B9"]);

            worksheet.Cells["C8:D8"].Merge = true;
            worksheet.Cells["C8:D8"].Value = "CHỨNG TỪ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["C8:D8"]);

            worksheet.Cells["C9"].Value = "SỐ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["C9"]);
            worksheet.Cells["D9"].Value = "NGÀY";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["D9"]);

            worksheet.Cells["E8:E9"].Merge = true;
            worksheet.Cells["E8:E9"].Value = "DIỄN GIẢI";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E8:E9"]);

            worksheet.Cells["F8:G8"].Merge = true;
            worksheet.Cells["F8:G8"].Value = "TÀI KHOẢN";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["F8:G8"]);

            worksheet.Cells["F9"].Value = "NỢ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["F9"]);
            worksheet.Cells["G9"].Value = "CÓ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G9"]);

            worksheet.Cells["H8:H9"].Merge = true;
            worksheet.Cells["H8:H9"].Value = "SỐ LƯỢNG";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["H8:H9"]);

            worksheet.Cells["I8:I9"].Merge = true;
            worksheet.Cells["I8:I9"].Value = "SỐ TIỀN";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["I8:I9"]);

            worksheet.Cells["J8:K8"].Merge = true;
            worksheet.Cells["J8:K8"].Value = "SỐ CHỨNG TỪ LIÊN QUAN";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["J8:K8"]);

            worksheet.Cells["J9"].Value = "HÓA ĐƠN";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["J9"]);
            worksheet.Cells["K9"].Value = "PHIẾU NHẬP";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["K9"]);


            int currentRowNo = 9, flagRowNo = 0;

            currentRowNo++;

            for (int i = 0; i < p.VoucherInfors.Count; i++)
            {
                flagRowNo = currentRowNo;

                VoucherInforItem _item = p.VoucherInfors[i];
                _item.Vouchers.ForEach(x =>
                {
                    worksheet.Cells[currentRowNo, 4].Value = x.OrginalBookDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[currentRowNo, 5].Value = x.VoucherDescription;
                    worksheet.Cells[currentRowNo, 6].Value = x.DebitCode;
                    worksheet.Cells[currentRowNo, 7].Value = x.CreditCode;
                    worksheet.Cells[currentRowNo, 8].Value = x.Quantity;
                    worksheet.Cells[currentRowNo, 9].Value = x.Amount;
                    worksheet.Cells[currentRowNo, 10].Value = x.RefInvoice;
                    worksheet.Cells[currentRowNo, 11].Value = x.RefVoucher;
                    currentRowNo++;
                });
                worksheet.Cells[flagRowNo, 8, currentRowNo - 1, 9].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                worksheet.Cells[flagRowNo, 1, currentRowNo - 1, 1].Merge = true;
                worksheet.Cells[flagRowNo, 1, currentRowNo - 1, 1].Value = (i + 1).ToString();

                worksheet.Cells[flagRowNo, 2, currentRowNo - 1, 2].Merge = true;
                worksheet.Cells[flagRowNo, 2, currentRowNo - 1, 2].Value = _item.ClosedDate.ToString("dd/MM/yyyy");

                worksheet.Cells[flagRowNo, 3, currentRowNo - 1, 3].Merge = true;
                worksheet.Cells[flagRowNo, 3, currentRowNo - 1, 3].Value = _item.VoucherNumber;
            }


            worksheet.Column(1).AutoFit(25);
            worksheet.Column(2).AutoFit(30);
            worksheet.Column(3).AutoFit(15);
            worksheet.Column(4).AutoFit(15);
            worksheet.Column(5).AutoFit(15);
            worksheet.Column(6).AutoFit(15);
            worksheet.Column(7).AutoFit(15);
            worksheet.Column(8).AutoFit(15);
            worksheet.Column(9).AutoFit(15);
            worksheet.Column(10).AutoFit(15);
            worksheet.Column(11).AutoFit(15);

            worksheet.SelectedRange["A1:K3"].Style.Font.Size = 12;
            worksheet.SelectedRange["A6:K6"].Style.Font.Size = 12;

            worksheet.SelectedRange["C5:I5"].Style.Font.Size = 16;
            worksheet.SelectedRange["C5:I5"].Style.Font.Bold = true;
            worksheet.Row(5).Height = 30;

            worksheet.SelectedRange["A8:K9"].Style.Font.Bold = true;
            worksheet.SelectedRange["A8:K9"].Style.Font.Size = 14;

            worksheet.Cells["A8:K9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.SelectedRange["A8:K9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 213, 180));
            worksheet.SelectedRange["A8:K9"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.SelectedRange["A10:K" + currentRowNo].Style.Font.Bold = false;
            worksheet.SelectedRange["A10:K" + currentRowNo].Style.Font.Size = 12;

            worksheet.SelectedRange["A1:K" + currentRowNo].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.SelectedRange["A4:K" + currentRowNo].Style.WrapText = true;

            worksheet.SelectedRange["C5:I6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.SelectedRange["A10:A" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            worksheet.SelectedRange["E10:E" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            worksheet.SelectedRange["B10:D" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.SelectedRange["F10:G" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.SelectedRange["G10:K" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.SelectedRange["H10:H" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            worksheet.SelectedRange["I10:I" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

            worksheet.SelectedRange["A10:A" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Value = "Người lập";
            worksheet.Cells[currentRowNo, 6, currentRowNo, 11].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 11].Value = "Kế toán trướng";

            currentRowNo+=2;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Value = !string.IsNullOrEmpty(param.VoteMaker) ? param.VoteMaker : string.Empty;

            worksheet.Cells[currentRowNo, 6, currentRowNo, 11].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 11].Value = p.isFillName ? p.ChiefAccountantName : string.Empty;

            worksheet.Cells[currentRowNo - 2, 1, currentRowNo, 11].Style.Font.Bold = true;
            worksheet.Cells[currentRowNo - 2, 1, currentRowNo, 11].Style.HorizontalAlignment =  ExcelHorizontalAlignment.Center;

            return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "BangKeChungTuGhiSo");
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion

    public IEnumerable<SelectListModel> GetListInvoiceTaxCode(DateTime from, DateTime to, int year, string voucherType = "PT")
    {
        var invoiceTaxCodes = _context.GetLedger(year, 2)
            .Where(x => string.IsNullOrEmpty(voucherType) || (x.Type == voucherType))
            .Where(x => x.OrginalBookDate.Value >= from
                            && x.OrginalBookDate.Value <= to
                            && (x.InvoiceTaxCode ?? "").Length > 0
                            ).Select(x => x.InvoiceTaxCode).Distinct().ToList();
        int i = 1;
        foreach(string invoice in invoiceTaxCodes)
        {
            SelectListModel item = new SelectListModel();
            item.Id = i;
            item.Type = 0;
            item.Code = "";
            item.Name = invoice;
            i++;
            yield return item;
        }
    }
    public IEnumerable<SelectListModel> GetListInvoiceNumber(DateTime from, DateTime to, int year, string voucherType = "PT")
    {
        var invoiceNumbers = _context.GetLedger(year, 2)
            .Where(x => string.IsNullOrEmpty(voucherType) || x.Type == voucherType)
            .Where(x => x.OrginalBookDate.Value >= from
                            && x.OrginalBookDate.Value <= to
                            ).Select(x => x.InvoiceNumber).Distinct().ToList();
        int i = 1;
        foreach (string invoice in invoiceNumbers)
        {
            SelectListModel item = new SelectListModel();
            item.Id = i;
            item.Type = 0;
            item.Code = "";
            item.Name = invoice;
            i++;
            yield return item;
        }
    }
}
