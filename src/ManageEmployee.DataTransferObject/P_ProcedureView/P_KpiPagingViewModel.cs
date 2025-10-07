namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_KpiPagingViewModel
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    public int Month { get; set; }
    public string? DepartmentName { get; set; }
    public string? P_ProcedureStatusName { get; set; }
    public DateTime? CreateAt { get; set; }
}
