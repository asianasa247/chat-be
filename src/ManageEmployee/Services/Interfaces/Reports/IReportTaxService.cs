using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface IReportTaxService
{
    string ExportXML();
    Task<string> ExportPDF(ReportTaxRequest request, bool isNoiBo, int year);
}
