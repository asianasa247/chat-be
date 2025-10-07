using Common.Extensions;
using DinkToPdf.Contracts;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.P_Procedures.ExpenditurePlans;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Services.P_ProcedureServices.ExpenditurePlanServices;
public class ExpenditurePlanExporter : IExpenditurePlanExporter
{
    private readonly ApplicationDbContext _context;
    private readonly ICompanyService _companyService;
    private readonly IProcedureExportHelper _procedureExportHelper;
    private readonly AppSettings _appSettings;
    private readonly IConverter _converterPDF;

    public ExpenditurePlanExporter(ApplicationDbContext context,
        ICompanyService companyService,
        IProcedureExportHelper procedureExportHelper,
        IOptions<AppSettings> appSettings,
        IConverter converterPDF)
    {
        _context = context;
        _companyService = companyService;
        _procedureExportHelper = procedureExportHelper;
        _appSettings = appSettings.Value;
        _converterPDF = converterPDF;
    }
    public async Task<string> Export(int expenditurePlanId)
    {
        var payment = await _context.ExpenditurePlans.FindAsync(expenditurePlanId);
        if (!payment.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        var company = await _companyService.GetCompany();

        var soThapPhan = $"N{company.MethodCalcExportPrice}";

        var paymentDetails = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == expenditurePlanId).ToListAsync();

        var resultSignHtml = await _procedureExportHelper.SignPlace(expenditurePlanId, nameof(ProcedureEnum.EXPENDITURE_PLAN));

        var resultHtml = GenerateExportTable(paymentDetails, soThapPhan);

        var paymentDate = payment.Date;
        var totalAmount = paymentDetails.Sum(x => x.ExpenditurePlanAmount) ?? 0;
        var values = new List<(string FieldName, string FieldValue)>
        {
            ("CompanyName", company.Name),
            ("CompanyAddress", company.Address),
            ("TotalAmount", totalAmount.FormatAmount(soThapPhan)),
            ("Day", $"{paymentDate.Day}"),
            ("Month", $"{paymentDate.Month}"),
            ("Year", $"{paymentDate.Year}"),
            ("Note", payment.Note),
            ("MST", company.MST),
            ("Phone", company.Phone),
            ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
        };

        var allText = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "KeHoachDuChi.html"
        );

        var allTextBuilder = new StringBuilder(allText);

        values.ForEach(x =>
        {
            allTextBuilder.Replace("{{{" + x.FieldName + "}}}", x.FieldValue);
        });

        allTextBuilder.Replace("##SIGN_REPLACE_PLACE##", resultSignHtml);
        allTextBuilder.Replace("##REPLACE_PLACE##", resultHtml);

        return ExcelHelpers.ConvertUseDink(
            allTextBuilder.ToString(),
            _converterPDF,
            Directory.GetCurrentDirectory(),
            "KeHoachDuChi"
        );
    }
    private string GenerateExportTable(List<ExpenditurePlanDetail> expenditurePlanDetails, string soThapPhan, bool isAdvanceSettlement = false)
    {
        var resultHtml = new StringBuilder();
        foreach (var expenditurePlanDetail in expenditurePlanDetails)
        {
            if (isAdvanceSettlement)
            {
                resultHtml.Append(GenerateExportRowAdvanceSettlement(expenditurePlanDetail, soThapPhan));
            }
            else
            {
                resultHtml.Append(GenerateExportRow(expenditurePlanDetail, soThapPhan));
            }
        }

        return resultHtml.ToString();
    }
    private string GenerateExportRow(ExpenditurePlanDetail expenditurePlanDetail, string soThapPhan)
    {
        string txt = $@"<tr>
                                <td class='txt-left'>{expenditurePlanDetail.FromProcedureCode}</td>
                                <td class='txt-left'>{expenditurePlanDetail.FromProcedureNote}</td>
                                <td class='txt-right'>{expenditurePlanDetail.ExpenditurePlanAmount?.FormatAmount(soThapPhan)}</td>                                
                                <td class='txt-right'>{expenditurePlanDetail.ApproveAmount?.FormatAmount(soThapPhan)}</td>
                                <td>{expenditurePlanDetail.Note}</td>
                            </tr>";
        return txt;
    }

    public async Task<string> ExportAdvanceSettlement(int expenditurePlanId)
    {
        var payment = await _context.ExpenditurePlans.FindAsync(expenditurePlanId);
        if (!payment.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        var company = await _companyService.GetCompany();

        var soThapPhan = $"N{company.MethodCalcExportPrice}";

        var paymentDetails = await _context.ExpenditurePlanDetails.Where(x => x.ExpenditurePlanId == expenditurePlanId).ToListAsync();

        var resultSignHtml = await _procedureExportHelper.SignPlace(expenditurePlanId, nameof(ProcedureEnum.EXPENDITURE_PLAN));

        var resultHtml = GenerateExportTable(paymentDetails, soThapPhan, true);

        var paymentDate = payment.Date;
        var totalAmount = paymentDetails.Sum(x => x.ExpenditurePlanAmount) ?? 0;
        var values = new List<(string FieldName, string FieldValue)>
        {
            ("CompanyName", company.Name),
            ("CompanyAddress", company.Address),
            ("TotalAmount", totalAmount.FormatAmount(soThapPhan)),
            ("Day", $"{paymentDate.Day}"),
            ("Month", $"{paymentDate.Month}"),
            ("Year", $"{paymentDate.Year}"),
            ("Note", payment.Note),
            ("MST", company.MST),
            ("Phone", company.Phone),
            ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
        };

        var allText = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "QuyetToanDuChi.html"
        );

        var allTextBuilder = new StringBuilder(allText);

        values.ForEach(x =>
        {
            allTextBuilder.Replace("{{{" + x.FieldName + "}}}", x.FieldValue);
        });

        allTextBuilder.Replace("##SIGN_REPLACE_PLACE##", resultSignHtml);
        allTextBuilder.Replace("##REPLACE_PLACE##", resultHtml);

        return ExcelHelpers.ConvertUseDink(
            allTextBuilder.ToString(),
            _converterPDF,
            Directory.GetCurrentDirectory(),
            "QuyetToanDuChi"
        );
    }
    private string GenerateExportRowAdvanceSettlement(ExpenditurePlanDetail expenditurePlanDetail, string soThapPhan)
    {
        string txt = $@"<tr>
                                <td class='txt-left'>{expenditurePlanDetail.FromProcedureCode}</td>
                                <td class='txt-left'>{expenditurePlanDetail.FromProcedureNote}</td>
                                <td class='txt-right'>{expenditurePlanDetail.ExpenditurePlanAmount?.FormatAmount(soThapPhan)}</td>                                 
                                <td class='txt-right'>{expenditurePlanDetail.ApproveAmount?.FormatAmount(soThapPhan)}</td>
                                <td class='txt-right'>{((expenditurePlanDetail.ExpenditurePlanAmount?? 0) - (expenditurePlanDetail.ApproveAmount ?? 0)).FormatAmount(soThapPhan)}</td> 
                                <td>{expenditurePlanDetail.Note}</td>
                            </tr>";
        return txt;
    }
}
