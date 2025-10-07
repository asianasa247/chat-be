using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface IPlanMissionCountryTaxService
{
    Task<string> ExportDataReport(PlanMissionCountryTaxParam request, bool isNoiBo, int year);
}
