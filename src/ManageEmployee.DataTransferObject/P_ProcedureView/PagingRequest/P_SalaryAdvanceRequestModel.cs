using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;

public class P_SalaryAdvanceRequestModel : PagingRequestModel
{
    public int? Month { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
}