namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_ProcedureViewModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Note { get; set; }
    public List<P_ProcedureStatusViewModel>? StatusItems { get; set; }
}
