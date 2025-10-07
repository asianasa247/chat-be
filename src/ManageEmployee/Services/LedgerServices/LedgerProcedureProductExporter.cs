using AutoMapper;
using Common.Extensions;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Generators;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;

namespace ManageEmployee.Services.LedgerServices;
public class LedgerProcedureProductExporter: ILedgerProcedureProductExporter
{
    private readonly IConverter _converterPdf;
    private readonly ILedgerProcedureProductService _ledgerProcedureProductService;
    private readonly IPdfGeneratorService _pdfGeneratorService;
    private readonly ILedgerProduceHelper _ledgerProduceHelper;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LedgerProcedureProductExporter(IConverter converterPdf, 
        ILedgerProcedureProductService ledgerProcedureProductService, 
        IPdfGeneratorService pdfGeneratorService, 
        ILedgerProduceHelper ledgerProduceHelper, 
        ApplicationDbContext context, 
        IMapper mapper)
    {
        _converterPdf = converterPdf;
        _ledgerProcedureProductService = ledgerProcedureProductService;
        _pdfGeneratorService = pdfGeneratorService;
        _ledgerProduceHelper = ledgerProduceHelper;
        _context = context;
        _mapper = mapper;
    }

    private async Task<string> ExportXK(int id)
    {
        var ledgerProduceExport = await _ledgerProcedureProductService.GetDetail(id);
        var details = await GetListDataPrint(id, "XK");

        if (details == null || !details.Any())
        {
            throw new Exception("Import Process Detail not found. Cannot export");
        }

        var firstDetail = details.First();

        string htmlContent = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "ExportProcessTemplate.html"
        );

        var htmlDoc = new HtmlDocument();

        htmlDoc.LoadHtml(htmlContent);
        // Get template section
        var rowTemplateElement = htmlDoc.GetElementbyId("table-row-template");

        if (rowTemplateElement == null)
        {
            throw new Exception("Table row template section not found. Cannot generate report");
        }

        // Remove template after get content
        rowTemplateElement.Remove();

        var mainSb = new StringBuilder(htmlDoc.DocumentNode.OuterHtml);

        // Mapping company information
        await _pdfGeneratorService.MappingCompany(mainSb);

        var taxPercent = firstDetail.TaxPercent;
        var header = await _context.Documents.FirstOrDefaultAsync(x => x.Code == "XK");

        // Mapping customer information
        var mappingVals = new Dictionary<string, string>
        {
            { nameof(ledgerProduceExport.ProcedureNumber), ledgerProduceExport.ProcedureNumber },
            { "OrginalBookDate", firstDetail.OrginalBookDate?.ToString("'Ngày' dd 'tháng' MM 'năm' yyyy") },
            { nameof(firstDetail.OrginalCompanyName), firstDetail.OrginalCompanyName },
            { nameof(firstDetail.OrginalAddress), firstDetail.OrginalAddress },
            { nameof(firstDetail.DebitCode), firstDetail.DebitCode },
            { nameof(firstDetail.CreditCode), firstDetail.CreditCode },
            { nameof(firstDetail.CreditWarehouseName), firstDetail.CreditWarehouseName },
            { nameof(firstDetail.OrginalDescription), firstDetail.OrginalDescription },
            { nameof(firstDetail.InvoiceTaxCode), firstDetail.InvoiceTaxCode },
            { nameof(firstDetail.DebitWarehouse), firstDetail.DebitWarehouse },
            { nameof(firstDetail.AttachVoucher), firstDetail.AttachVoucher },
            { "TaxPercent", taxPercent.ToString("n0") },
            { "header", header?.Name },
        };
        foreach (var val in mappingVals)
        {
            mainSb.Replace($"[{val.Key}]", val.Value);
        }

        // Mapping table detail section
        var tblDetailSb = new StringBuilder();

        var rIndex = 1;
        double subTotal = 0;
        double taxTotal = 0;

        foreach (var item in details)
        {
            var rowSb = new StringBuilder(rowTemplateElement.InnerHtml)
                .Replace("[Index]", rIndex.ToString())
                .Replace("[Code]", item.CreditDetailCodeFirst.DefaultIfNullOrEmpty(
                    item.CreditDetailCodeFirst.DefaultIfNullOrEmpty(item.CreditCode))
                )
                .Replace("[Name]", item.CreditDetailCodeSecondName.DefaultIfNullOrEmpty(
                    item.CreditDetailCodeFirstName.DefaultIfNullOrEmpty(item.CreditCodeName))
                )
                .Replace("[Quantity]", item.Quantity.ToString("n2"))
                .Replace("[StockUnit]", item.StockUnit)
                .Replace("[WarehouseName]", item.CreditWarehouseName)
                .Replace("[OrginalDescription]", item.OrginalDescription)
                .Replace("[UnitPrice]", item.UnitPrice.ToString("n0"))
                .Replace("[Amount]", item.Amount.ToString("n0"));
            tblDetailSb.Append(rowSb);
            subTotal += item.Amount;
            taxTotal += item.Amount * taxPercent / 100;
            rIndex++;
        }

        double total = subTotal + taxTotal;

        // Remove template section after mapping
        rowTemplateElement.Remove();

        mainSb.Replace("[TABLE_BODY_SECTION]", tblDetailSb.ToString())
            .Replace("[SubTotal]", subTotal.ToString("n0"))
            .Replace("[TaxTotal]", taxTotal.ToString("n0"))
            .Replace("[Total]", total.ToString("n0"))
            .Replace("[AmountInWords]", total.ConvertFromDecimal())
            .Replace("[SubAmountInWords]", subTotal.ConvertFromDecimal());
        var orginalCompanyName = details.FirstOrDefault()?.OrginalCompanyName;

        var signingHtml = await _ledgerProduceHelper.SignPlace(orginalCompanyName, id, nameof(ProcedureEnum.LEDGER_EXPORT));

        // Mapping Signing section
        mainSb.Replace("[SIGN_REPLACE_PLACE]", signingHtml);

        var html = mainSb.ToString();
        var generatedFileName = ExcelHelpers.ConvertUseDink(html, _converterPdf, Directory.GetCurrentDirectory(),
            "ExportProcessProduce");
        return generatedFileName;
    }

    private async Task<string> ExportNK(int id)
    {
        var ledgerProduceImport = await _ledgerProcedureProductService.GetDetail(id);

        var details = await GetListDataPrint(id, "NK");

        if (details == null || !details.Any())
        {
            throw new Exception("Import Process Detail not found. Cannot export");
        }
        var firstDetail = details.First();

        string htmlContent = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "ImportProcessTemplate.html"
        );

        var htmlDoc = new HtmlDocument();

        htmlDoc.LoadHtml(htmlContent);
        // Get template section
        var rowTemplateElement = htmlDoc.GetElementbyId("table-row-template");

        // Remove template after get content
        rowTemplateElement.Remove();

        var mainSb = new StringBuilder(htmlDoc.DocumentNode.OuterHtml);

        // Mapping company information
        await _pdfGeneratorService.MappingCompany(mainSb);

        // Mapping customer information
        var taxPercent = firstDetail.TaxPercent;
        var header = await _context.Documents.FirstOrDefaultAsync(x => x.Code == "NK");
        var mappingVals = new Dictionary<string, string>
        {
            { nameof(ledgerProduceImport.ProcedureNumber), ledgerProduceImport.ProcedureNumber },
            { "OrginalBookDate", firstDetail.OrginalBookDate?.ToString("'Ngày' dd 'tháng' MM 'năm' yyyy") },
            { nameof(firstDetail.OrginalCompanyName), firstDetail.OrginalCompanyName },
            { nameof(firstDetail.OrginalAddress), firstDetail.OrginalAddress },
            { nameof(firstDetail.DebitCode), firstDetail.DebitCode },
            { nameof(firstDetail.CreditCode), firstDetail.CreditCode },
            { nameof(firstDetail.CreditWarehouseName), firstDetail.CreditWarehouseName },
            { nameof(firstDetail.OrginalDescription), firstDetail.OrginalDescription },
            { nameof(firstDetail.AttachVoucher), firstDetail.AttachVoucher },
            { "header", header?.Name },
        };
        foreach (var val in mappingVals)
        {
            mainSb.Replace($"[{val.Key}]", val.Value);
        }

        // Mapping table detail section
        var tblDetailSb = new StringBuilder();

        var rIndex = 1;
        double subTotal = 0;
        double taxTotal = 0;
        foreach (var item in details)
        {
            var rowSb = new StringBuilder(rowTemplateElement.InnerHtml)
                .Replace("[Index]", rIndex.ToString())
                .Replace("[Code]", item.DebitDetailCodeSecond.DefaultIfNullOrEmpty(
                    item.DebitDetailCodeFirst.DefaultIfNullOrEmpty(item.DebitCode))
                )
                .Replace("[Name]", item.DebitDetailCodeSecondName.DefaultIfNullOrEmpty(
                    item.DebitDetailCodeFirstName.DefaultIfNullOrEmpty(item.DebitCodeName))
                )
                .Replace("[Quantity]", item.Quantity.ToString("n2"))
                .Replace("[StockUnit]", item.StockUnit)
                .Replace("[OrginalDescription]", item.OrginalDescription)
                .Replace("[UnitPrice]", item.UnitPrice.ToString("n0"))
                .Replace("[Amount]", item.Amount.ToString("n0"));
            tblDetailSb.Append(rowSb);
            subTotal += item.Amount;
            taxTotal += item.Amount * taxPercent / 100;
            rIndex++;
        }
        double total = subTotal + taxTotal;

        // Remove template section after mapping
        rowTemplateElement.Remove();

        mainSb.Replace("[TABLE_BODY_SECTION]", tblDetailSb.ToString())
            .Replace("[SubTotal]", subTotal.ToString("n0"))
            .Replace("[TaxTotal]", taxTotal.ToString("n0"))
            .Replace("[Total]", total.ToString("n0"))
            .Replace("[AmountInWords]", total.ConvertFromDecimal())
            .Replace("[SubAmountInWords]", subTotal.ConvertFromDecimal());
        var orginalCompanyName = details.FirstOrDefault()?.OrginalCompanyName;
        var signingHtml = await _ledgerProduceHelper.SignPlace(orginalCompanyName, id, nameof(ProcedureEnum.LEDGER_IMPORT));

        // Mapping Signing section
        mainSb.Replace("[SIGN_REPLACE_PLACE]", signingHtml);

        var html = mainSb.ToString();
        var generatedFileName = ExcelHelpers.ConvertUseDink(html, _converterPdf, Directory.GetCurrentDirectory(),
            "ImportProcessProduce");
        return generatedFileName;
    }
    private async Task<string> ExportPT(int id)
    {
        // Lấy thông tin chi tiết của phiếu thu
        var receiptDetail = await _ledgerProcedureProductService.GetDetail(id);
        var details = await GetListDataPrint(id, "PT");

        if (details == null || !details.Any())
        {
            throw new Exception("Phiếu thu không tìm thấy. Không thể xuất phiếu.");
        }

        var firstDetail = details.First();

        // Đọc nội dung của mẫu HTML
        string htmlContent = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "ReceiptsProcessTemplate.html"
        );

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // Lấy phần mẫu của bảng
        var rowTemplateElement = htmlDoc.GetElementbyId("table-row-template");

        if (rowTemplateElement == null)
        {
            throw new Exception("Không tìm thấy mẫu bảng. Không thể tạo báo cáo.");
        }

        //Loại bỏ mẫu bảng sau khi lấy nội dung
        rowTemplateElement.Remove();

        var mainSb = new StringBuilder(htmlDoc.DocumentNode.OuterHtml);

        // Mapping thông tin công ty
        await _pdfGeneratorService.MappingCompany(mainSb);

        // Lấy các thông tin cần thiết từ chi tiết phiếu thu
        var header = await _context.Documents.FirstOrDefaultAsync(x => x.Code == "PT");

        var mappingVals = new Dictionary<string, string>
        {
            { "OrginalBookDate", firstDetail.OrginalBookDate?.ToString("'Ngày' dd 'tháng' MM 'năm' yyyy") },
            { "OrginalCompanyName", firstDetail.OrginalCompanyName },
            { "OrginalAddress", firstDetail.OrginalAddress },
            { "OrginalDescription", firstDetail.OrginalDescription },
            { "CreditCode", firstDetail.CreditCode },
            { "Amount", firstDetail.Amount.ToString("n0") },
            { "TotalAmount", firstDetail.Amount.ToString("n0") },
            { "AmountInWords", firstDetail.Amount.ConvertFromDecimal() },
            { "header", header?.Name },
            { "ProcedureNumber", receiptDetail?.ProcedureNumber },
            { "OrginalVoucherNumber", firstDetail.OrginalVoucherNumber },
        };

        // Thay thế các giá trị vào trong mẫu HTML
        foreach (var val in mappingVals)
        {
            mainSb.Replace($"[{val.Key}]", val.Value);
        }

        // Thay thế thông tin chi tiết trong bảng
        var tblDetailSb = new StringBuilder();

        var rIndex = 1;
        double subTotal = 0;
        foreach (var item in details)
        {
            var rowSb = new StringBuilder(rowTemplateElement.InnerHtml)
                .Replace("[Index]", rIndex.ToString())
                .Replace("[CreditCode]", item.CreditCode ?? "")
                .Replace("[Amount]", item.Amount.ToString("n0"));
            tblDetailSb.Append(rowSb);
            subTotal += item.Amount;
            rIndex++;
        }

        // Loại bỏ mẫu bảng sau khi thay thế
        rowTemplateElement.Remove();

        // Thay thế các thông tin tổng hợp vào trong mẫu
        mainSb.Replace("[TABLE_BODY_SECTION]", tblDetailSb.ToString())
             .Replace("[SubTotal]", subTotal.ToString("n0"))
             .Replace("[Total]", subTotal.ToString("n0"))
             .Replace("[AmountInWords]", subTotal.ConvertFromDecimal());

        // Thêm phần chữ ký
        var signingHtml = await _ledgerProduceHelper.SignPlace(firstDetail.OrginalCompanyName, id, nameof(ProcedureEnum.LEDGER_EXPORT));

        // Mapping phần chữ ký
        mainSb.Replace("[SIGN_REPLACE_PLACE]", signingHtml);

        var html = mainSb.ToString();

        // Tạo file PDF từ HTML đã chỉnh sửa
        var generatedFileName = ExcelHelpers.ConvertUseDink(html, _converterPdf, Directory.GetCurrentDirectory(),
             "ReceiptsProcessTemplate");

        return generatedFileName;
    }

    private async Task<string> ExportPC(int id)
    {
        // Lấy thông tin chi tiết của phiếu thu
        var receiptDetail = await _ledgerProcedureProductService.GetDetail(id);
        var details = await GetListDataPrint(id, "PC");

        if (details == null || !details.Any())
        {
            throw new Exception("Phiếu chi không tìm thấy. Không thể xuất phiếu.");
        }

        var firstDetail = details.First();

        // Đọc nội dung của mẫu HTML
        string htmlContent = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "ReceiptsProcessTemplate.html"
        );

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // Lấy phần mẫu của bảng
        var rowTemplateElement = htmlDoc.GetElementbyId("table-row-template");

        if (rowTemplateElement == null)
        {
            throw new Exception("Không tìm thấy mẫu bảng. Không thể tạo báo cáo.");
        }

        //Loại bỏ mẫu bảng sau khi lấy nội dung
        rowTemplateElement.Remove();

        var mainSb = new StringBuilder(htmlDoc.DocumentNode.OuterHtml);

        // Mapping thông tin công ty
        await _pdfGeneratorService.MappingCompany(mainSb);

        // Lấy các thông tin cần thiết từ chi tiết phiếu thu
        var header = await _context.Documents.FirstOrDefaultAsync(x => x.Code == "PC");

        var mappingVals = new Dictionary<string, string>
        {
            { "OrginalBookDate", firstDetail.OrginalBookDate?.ToString("'Ngày' dd 'tháng' MM 'năm' yyyy") },
            { "OrginalCompanyName", firstDetail.OrginalCompanyName },
            { "OrginalAddress", firstDetail.OrginalAddress },
            { "OrginalDescription", firstDetail.OrginalDescription },
            { "CreditCode", firstDetail.CreditCode },
            { "Amount", firstDetail.Amount.ToString("n0") },
            { "TotalAmount", firstDetail.Amount.ToString("n0") },
            { "AmountInWords", firstDetail.Amount.ConvertFromDecimal() },
            { "header", header?.Name },
            { "ProcedureNumber", receiptDetail?.ProcedureNumber },
            { "OrginalVoucherNumber", firstDetail.OrginalVoucherNumber },
        };

        // Thay thế các giá trị vào trong mẫu HTML
        foreach (var val in mappingVals)
        {
            mainSb.Replace($"[{val.Key}]", val.Value);
        }

        // Thay thế thông tin chi tiết trong bảng
        var tblDetailSb = new StringBuilder();

        var rIndex = 1;
        double subTotal = 0;
        foreach (var item in details)
        {
            var rowSb = new StringBuilder(rowTemplateElement.InnerHtml)
                .Replace("[Index]", rIndex.ToString())
                .Replace("[CreditCode]", item.CreditCode ?? "")
                .Replace("[Amount]", item.Amount.ToString("n0"));
            tblDetailSb.Append(rowSb);
            subTotal += item.Amount;
            rIndex++;
        }

        // Loại bỏ mẫu bảng sau khi thay thế
        rowTemplateElement.Remove();

        // Thay thế các thông tin tổng hợp vào trong mẫu
        mainSb.Replace("[TABLE_BODY_SECTION]", tblDetailSb.ToString())
             .Replace("[SubTotal]", subTotal.ToString("n0"))
             .Replace("[Total]", subTotal.ToString("n0"))
             .Replace("[AmountInWords]", subTotal.ConvertFromDecimal());

        // Thêm phần chữ ký
        var signingHtml = await _ledgerProduceHelper.SignPlace(firstDetail.OrginalCompanyName, id, nameof(ProcedureEnum.LEDGER_EXPORT));

        // Mapping phần chữ ký
        mainSb.Replace("[SIGN_REPLACE_PLACE]", signingHtml);

        var html = mainSb.ToString();

        // Tạo file PDF từ HTML đã chỉnh sửa
        var generatedFileName = ExcelHelpers.ConvertUseDink(html, _converterPdf, Directory.GetCurrentDirectory(),
             "PaymentsProcessTemplate");

        return generatedFileName;
    }

    public async Task<string> Export(int id)
    {
        var ledgerProduceExport = await _context.LedgerProcedureProducts.FindAsync(id);

        var fileName = "";
        if (ledgerProduceExport.Type == "NK")
        {
            fileName = await ExportNK(id);
        }
        else if (ledgerProduceExport.Type == "XK")
        {
            fileName = await ExportXK(id);
        }
        else if (ledgerProduceExport.Type == "PT")
        {
            fileName = await ExportPT(id);
        }
        else if (ledgerProduceExport.Type == "PC")
        {
            fileName = await ExportPC(id);
        }
        return fileName;
    }
    private async Task<List<LedgerPrint>> GetListDataPrint(int id, string type)
    {
        //var ledgerProduceExport = await _context.LedgerProcedureProducts.FindAsync(id);
        // if (!ledgerProduceExport.IsFinished)
        // {
        //     throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        // }
        var ledgerProduceExportDetails = await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == id).ToListAsync();

        var ledgerPrints = new List<LedgerPrint>();

        foreach (var ledger in ledgerProduceExportDetails)
        {
            var ledgerPrint = _mapper.Map<LedgerPrint>(ledger);
            var taxRate = await _context.TaxRates.FirstOrDefaultAsync(x => x.Code == ledger.InvoiceCode);
            ledgerPrint.TaxPercent = taxRate?.Percent ?? 0;

            

            ledgerPrints.Add(ledgerPrint);
        }
        return ledgerPrints;
    }

}
