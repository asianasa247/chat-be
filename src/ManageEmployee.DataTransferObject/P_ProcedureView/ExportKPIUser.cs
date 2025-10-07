namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class ExportKPIUser
{
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public int UserId { get; set; }
    public double? PointKpi { get; set; }
    public double? Point { get; set; }
    public double? Percent { get; set; }
    public string? DepartmentName { get; set; }
}
