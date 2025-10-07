using DinkToPdf.Contracts;
using System.Xml.Serialization;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Reports;
using ManageEmployee.Services.Interfaces.Companies;
using Common.Helpers;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.XML;

namespace ManageEmployee.Services.Reports;
public class ReportTaxService : IReportTaxService
{
    private readonly IConverter _converterPDF;
    private readonly ICompanyService _companyService;
    private readonly ApplicationDbContext _context;
    public ReportTaxService(ApplicationDbContext context
, IConverter converterPDF,
ICompanyService companyService)
    {
        _context = context;
        _converterPDF = converterPDF;
        _companyService = companyService;
    }
    public HSoThueDTu InitData()
    {
        string _template = "ReportTax.json",
                _folderPath = @"Uploads\Json",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
        HSoThueDTu? _objBase = _allText.Deserialize<HSoThueDTu>();
        return _objBase!;
    }
    public string ExportXML()
    {
        HSoThueDTu oObject = InitData();
        XmlSerializer xmlSerializer = new XmlSerializer(oObject.GetType());


        string path = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\XML\\");
        string _fileMapServer = $"BaoCaoThue_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xml";
        string _pathSave = Path.Combine(path, _fileMapServer);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        using (FileStream file = File.Create(_pathSave))
        {
            xmlSerializer.Serialize(file, oObject);
            file.Close();
        }

        return _fileMapServer;
    }
    public async Task<string> ExportPDF(ReportTaxRequest request, bool isNoiBo, int year)
    {
        var company = await _companyService.GetCompany();

        var modelCompanyInfo = new VoucherReportViewModel()
        {
            Company = company.Name,
            Address = company.Address,
            TaxId = company.MST,
            ChiefAccountantName = company.NameOfChiefAccountant,
            NoteChiefAccountantName = company.NoteOfChiefAccountant,
            CEOOfName = company.NameOfCEO,
            CEOOfNote = company.NoteOfCEO,
            MethodCalcExportPrice = company.MethodCalcExportPrice,
        };
        HSoThueDTu oObject = InitData();
        string _path = string.Empty;

        switch (request.FileType)
        {
            case "html":
                _path = await ConvertToHTML(oObject, modelCompanyInfo, request, year);
                break;
            case "pdf":
                _path = await ConvertToPDFFile(oObject, modelCompanyInfo, request, year);
                break;
        }
        return _path;
    }

    private async Task<string> ConvertToHTML(HSoThueDTu oObject, VoucherReportViewModel p, ReportTaxRequest request, int year)
    {
        try
        {
            if (p == null) return string.Empty;
            if (request.FromMonth == null) request.FromMonth = 0;
            if (request.ToMonth == null) request.ToMonth = 0;
            if (request.FromDate == null) request.FromDate = DateTime.Now;
            if (request.ToDate == null) request.ToDate = DateTime.Now;

            string _template = "ToKhaiThue.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "LoaiChungTu", p.Type+" - "+ p.TypeName },
                { "SoCT", string.Empty },
                { "MaxNgay", string.Empty },
                { "TuThang", ((request.FromMonth > 0 && request.ToMonth > 0) ? ((int)request.FromMonth) : ((DateTime)request.FromDate).Month ).ToString("D2")   },
                { "DenThang", ( (request.FromMonth > 0 && request.ToMonth > 0) ? ((int)request.ToMonth) : ((DateTime)request.ToDate).Month ).ToString("D2") },
                { "Ngay", "......" },
                { "Thang", "......" },
                { "NamSign", "......" },
                { "KeToanTruong_CV", p.NoteChiefAccountantName},
                { "GiamDoc_CV", p.CEOOfNote},
            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            if (oObject != null)
            {
                var debit1331 = await _context.GetLedger(year).Where(x => x.DebitCode == "1331").SumAsync(x => x.Amount);
                var debit33311 = await _context.GetLedger(year).Where(x => x.DebitCode == "33311").SumAsync(x => x.Amount);
                var creditt33311 = await _context.GetLedger(year).Where(x => x.CreditCode == "33311").SumAsync(x => x.Amount);


                var debit1331InvoiceOVoucherNumbers = await _context.GetLedger(year).Where(x => x.DebitCode == "1331").Select(x => x.OrginalVoucherNumber).ToListAsync();
                var debit1331Invoice23 = await _context.GetLedger(year).Where(x => debit1331InvoiceOVoucherNumbers.Contains(x.OrginalVoucherNumber) && x.InvoiceCode.Contains("V")).SumAsync(x => x.Amount);

                var debit1332InvoiceOVoucherNumbers = await _context.GetLedger(year).Where(x => x.DebitCode == "1332").Select(x => x.OrginalVoucherNumber).ToListAsync();
                var debit1332Invoice23a = await _context.GetLedger(year).Where(x => debit1332InvoiceOVoucherNumbers.Contains(x.OrginalVoucherNumber) && x.InvoiceCode.Contains("V")).SumAsync(x => x.Amount);


                var debit1331Invoice24 = await _context.GetLedger(year).Where(x => x.DebitCode == "1331" && !string.IsNullOrEmpty(x.InvoiceCode) && x.InvoiceCode.Contains("V")).SumAsync(x => x.Amount);
                var debit1332Invoice24a = await _context.GetLedger(year).Where(x => x.DebitCode == "1332" && !string.IsNullOrEmpty(x.InvoiceCode) && x.InvoiceCode.Contains("V")).SumAsync(x => x.Amount);

                var amount23 = debit1331Invoice23 - debit1331Invoice24;
                var amount23a = debit1332Invoice23a - debit1332Invoice24a;


                var amountR01OVoucherNumbers = await _context.GetLedger(year).Where(x => x.InvoiceCode == "R01").Select(x => x.OrginalVoucherNumber).ToListAsync();
                var amountR01 = await _context.GetLedger(year).Where(x => amountR01OVoucherNumbers.Contains(x.OrginalVoucherNumber)).SumAsync(x => x.Amount);


                var amountR02OVoucherNumbers = await _context.GetLedger(year).Where(x => x.InvoiceCode == "R02").Select(x => x.OrginalVoucherNumber).ToListAsync();
                var amountR02_29 = await _context.GetLedger(year).Where(x => amountR02OVoucherNumbers.Contains(x.OrginalVoucherNumber)).SumAsync(x => x.Amount);

                var amountR03OVoucherNumbers = await _context.GetLedger(year).Where(x => x.InvoiceCode == "R03").Select(x => x.OrginalVoucherNumber).ToListAsync();
                var amountR03_30 = await _context.GetLedger(year).Where(x => amountR03OVoucherNumbers.Contains(x.OrginalVoucherNumber)).SumAsync(x => x.Amount);

                var amountR03_31 = await _context.GetLedger(year).Where(x => x.InvoiceCode == "R03").SumAsync(x => x.Amount);

                var amountR04OVoucherNumbers = await _context.GetLedger(year).Where(x => x.InvoiceCode == "R04").Select(x => x.OrginalVoucherNumber).ToListAsync();
                var amountR04_32 = await _context.GetLedger(year).Where(x => amountR04OVoucherNumbers.Contains(x.OrginalVoucherNumber)).SumAsync(x => x.Amount);

                var amountR04_33 = await _context.GetLedger(year).Where(x => x.InvoiceCode == "R04").SumAsync(x => x.Amount);

                var amount27 = amountR02_29 + amountR03_30 + amountR04_32;
                var amount28 = amountR03_31 + amountR04_33;


                var amount34 = amountR01 + amount27;
                var amount35 = amountR01;
                var amount36 = amount35 - debit1332Invoice24a;

                var amount22 = debit1331 - (debit33311 - creditt33311);
                var amount40a = amount36 - amount22;
                var amount40b = 0;
                var amount40 = amount40a - amount40b;
                var amount41 = amount36 - amount22;
                var amount42 = amount41;
                var amount43 = amountR01;
                string soThapPhan = "N" + p.MethodCalcExportPrice;

                IDictionary<string, string> v_value = new Dictionary<string, string>
                {
                    { "VALUE_22", String.Format("{0:" + soThapPhan + "}", amount22)},
                    { "VALUE_23", String.Format("{0:" + soThapPhan + "}", amount23) },
                    { "VALUE_23a", String.Format("{0:" + soThapPhan + "}", amount23a) },
                    { "VALUE_24", String.Format("{0:" + soThapPhan + "}", debit1331Invoice24) },
                    { "VALUE_24a", String.Format("{0:" + soThapPhan + "}", debit1332Invoice24a) },
                    { "VALUE_25", String.Format("{0:" + soThapPhan + "}", debit1332Invoice24a) },
                    { "VALUE_26", String.Format("{0:" + soThapPhan + "}", amountR01) },
                    { "VALUE_27", String.Format("{0:" + soThapPhan + "}", amount27) },
                    { "VALUE_28", String.Format("{0:" + soThapPhan + "}", amount28) },
                    { "VALUE_29", String.Format("{0:" + soThapPhan + "}", amountR02_29) },
                    { "VALUE_30", String.Format("{0:" + soThapPhan + "}", amountR03_30) },
                    { "VALUE_31", String.Format("{0:" + soThapPhan + "}", amountR03_31) },
                    { "VALUE_32", String.Format("{0:" + soThapPhan + "}", amountR04_32) },
                    { "VALUE_33", String.Format("{0:" + soThapPhan + "}", amountR04_33) },
                    { "VALUE_34", String.Format("{0:" + soThapPhan + "}", amount34) },
                    { "VALUE_35", String.Format("{0:" + soThapPhan + "}", amount35) },
                    { "VALUE_36", String.Format("{0:" + soThapPhan + "}", amount36) },
                    { "VALUE_40a", String.Format("{0:" + soThapPhan + "}", amount40a) },
                    { "VALUE_40b", String.Format("{0:" + soThapPhan + "}", amount40b) },
                    { "VALUE_40", String.Format("{0:" + soThapPhan + "}", amount40) },
                    { "VALUE_41", String.Format("{0:" + soThapPhan + "}", amount41) },
                    { "VALUE_42", String.Format("{0:" + soThapPhan + "}", amount42) },
                    { "VALUE_43", String.Format("{0:" + soThapPhan + "}", amount43) },
                };

                v_value.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_value[x]));


            }
            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<string> ConvertToPDFFile(HSoThueDTu oObject, VoucherReportViewModel p, ReportTaxRequest request, int year)
    {
        try
        {
            string _allText = await ConvertToHTML(oObject, p, request, year);
            return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "ToKhaiThue");
        }
        catch
        {
            return string.Empty;
        }
    }
}
