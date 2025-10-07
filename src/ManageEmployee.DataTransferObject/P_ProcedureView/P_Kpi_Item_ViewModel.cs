namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_Kpi_Item_ViewModel
{
    public int Id { get; set; }
    public int P_KpiId { get; set; }
    public int? UserId { get; set; }
    public string? DepartmentName { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public double? PointKpi { get; set; }
    public double? Point { get; set; }
    public double? Percent { get; set; }
}
