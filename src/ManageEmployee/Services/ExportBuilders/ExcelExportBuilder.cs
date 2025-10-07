using Common.Extensions;
using DinkToPdf.Contracts;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using System.Text;

namespace ManageEmployee.Services.ExportBuilders;

public class ExcelExportBuilder : IExportBuilder
{
    private readonly AppSettings _appSettings;
    private readonly IConverter _converterPDF;
    private readonly ICompanyService _companyService;
    private readonly IProcedureExportHelper _procedureExportHelper;
    public ExcelExportBuilder(
        ICompanyService companyService,
        AppSettings appSettings,
        IConverter converterPDF,
        IProcedureExportHelper procedureExportHelper)
    {
        _companyService = companyService;
        _appSettings = appSettings;
        _converterPDF = converterPDF;
        _procedureExportHelper = procedureExportHelper;
    }

    public async Task<string> Export<T>(ExportRequestModel<T> request) where T : class
    {
        var resultSignHtml = await _procedureExportHelper.SignPlace(request.ProcedureId, request.ProcedureCode);

        var company = await _companyService.GetCompany();
        var soThapPhan = $"N{company.MethodCalcExportPrice}";

        var resultHtml = GenerateExportTable(request, soThapPhan);

        var values = GetFieldValues(request, company);

        var paths = new List<string> 
        {
             Directory.GetCurrentDirectory()
        };

        paths.AddRange(request.TemplatePath);

        var allText = await FileExtension.ReadContent(paths.ToArray());

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
            request.PrefixFile
        );
    }

    private List<ExportFieldModel> GetFieldValues<T>(ExportRequestModel<T> request, Company company) where T : class
    {
        var values = new List<ExportFieldModel>
        {
           new ("CompanyName", company.Name),
           new ("CompanyAddress", company.Address),
           new ("MST", company.MST),
           new ("Phone", company.Phone),
           new ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
        };

        request.Fields.ForEach(x =>
        {
            var item = values.FirstOrDefault(value => value.FieldName == x.FieldName);
            if(item != null)
            {
                values.Remove(item);
            }

            values.Add(x);
        });

        return values;
    }

    private string GenerateExportTable<T>(ExportRequestModel<T> request, string soThapPhan) where T : class
    {
        var resultHtml = new StringBuilder();
        int index = 0;
        foreach (var item in request.Data)
        {
            index++;
            resultHtml.Append(request.GenerateExportRow(item, index, soThapPhan));
        }

        return resultHtml.ToString();
    }
}
