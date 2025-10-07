namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class ReportKpiAllItem
{
    public string? Name { get; set; }
    public double? PointKpi { get; set; }
    public double? Point { get; set; }
    public double? Percent { get; set; }
    public List<ReportKpiAllItem>? Items { get; set; }

}
