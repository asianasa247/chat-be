namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_SalaryAdvancePagingViewModel
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? CreateAt { get; set; }
    public string? P_ProcedureStatusName { get; set; }
    public bool IsFinish { get; set; }
}
