using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IP_KpiService
{
    Task<PagingResult<P_KpiPagingViewModel>> GetPaging(P_KpiRequestModel param);
    P_KpiViewModel GetById(int id);
    Task<string> Create(P_KpiViewModel param, int userId);
    Task<string> Update(P_KpiViewModel param, int userId);
    Task<string> Delete(int id);
    string GetProcedureNumber();
    Task<string> Accept(P_KpiViewModel param, int userId);
    Task<IEnumerable<ExportKPIUser>> ReportKPI(int? UserId, int? DepartmentId, int? BranchId, int Month);
    IEnumerable<P_Kpi_Item_ViewModel> GetAllUserActive(int? DepartmentId);
    Task<string> ExportExcel_Report_Kpi(int? UserId, int? DepartmentId, int? BranchId, int Month);
    Task<ReportKpiAllItem> ReportKpiAll(int month);
    Task<double> GetPointForUser(int userId, int month);
}
