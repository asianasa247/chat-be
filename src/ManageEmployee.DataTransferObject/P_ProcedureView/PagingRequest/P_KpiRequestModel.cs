using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;

public class P_KpiRequestModel : PagingRequestModel
{
    public int? Month { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
    public int? UserId { get; set; }
}