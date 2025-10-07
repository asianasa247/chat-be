namespace ManageEmployee.DataTransferObject.P_ProcedureView;
public class P_KpiViewModel
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    public int Month { get; set; }
    public int? DepartmentId { get; set; }
    public int? P_ProcedureStatusId { get; set; }
    public string? P_ProcedureStatusName { get; set; }
    public List<P_Kpi_Item_ViewModel>? Items { get; set; }
}
