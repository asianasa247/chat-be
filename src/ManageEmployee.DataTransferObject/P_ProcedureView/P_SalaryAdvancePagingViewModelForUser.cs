namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_SalaryAdvancePagingViewModelForUser
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    public string? BranchName { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime? Date { get; set; }
    public double? Value { get; set; }
    public string? P_ProcedureStatusName { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
}
