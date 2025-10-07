using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Options;
using System.Text;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Generators;

namespace ManageEmployee.Services;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly IConverter _converter;
    private readonly ICompanyService _companyService;
    private readonly AppSettings _appSettings;

    public PdfGeneratorService(
        IConverter converter,
        ICompanyService companyService,
        IOptions<AppSettings> appSettings)
    {
        _converter = converter;
        _companyService = companyService;
        _appSettings = appSettings.Value;
    }

    public async Task<byte[]> GeneratePdf(string html, string type)
    {
        string headerTemplatePath = "";
        if (type == "baogia")
        {
            headerTemplatePath = await GetHtmlTemplate("BaoGiaHeader");
        }
        else
        {
            headerTemplatePath = await GetHtmlTemplate("BillHeader");
        }
        
        // Create a new PdfDocument object.
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 40, Bottom = 20 },
            DocumentTitle = "Bill",
        };

        var objectSettings = new ObjectSettings
        {
            HtmlContent = html,
            WebSettings = { DefaultEncoding = "utf-8" },
            HeaderSettings = { HtmUrl = headerTemplatePath },
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };
        var bytes = _converter.Convert(pdf);

        // Remove temporary file after generate success
        File.Delete(headerTemplatePath);

        return bytes;
    }

    private async Task<string> GetHtmlTemplate(string templateName)
    {
        string htmlTemplateFolder = "Uploads\\Html";
        var headerPath = Path.Combine(Directory.GetCurrentDirectory(), htmlTemplateFolder, $"{templateName}.html");

        StringBuilder sb = new StringBuilder(File.ReadAllText(headerPath));
        await MappingCompany(sb);
        var newFilePath = Path.Combine(Directory.GetCurrentDirectory(), htmlTemplateFolder, $"{templateName}_Temp_{DateTime.Now:MMddyyyyhhmmss}.html");
        File.WriteAllText(newFilePath, sb.ToString());
        return newFilePath;
    }

    public async Task MappingCompany(StringBuilder sb)
    {
        var company = await _companyService.GetCompany();
        sb.Replace("[CompanyName]", company?.Name);
        sb.Replace("[CompanyAddress]", company?.Address);
        sb.Replace("[Logo]", $"{_appSettings.UrlHost}{company?.FileLogo}");
        sb.Replace("[CompanyTax]", company.MST);
        sb.Replace("[Phone]", company.Phone);
    }
}