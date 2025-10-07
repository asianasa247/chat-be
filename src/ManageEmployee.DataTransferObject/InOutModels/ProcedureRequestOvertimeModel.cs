namespace ManageEmployee.DataTransferObject.InOutModels;

public class ProcedureRequestOvertimeModel
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public List<int>? UserIds { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsFinish { get; set; }
    public int SymbolId { get; set; }
    public double Coefficient { get; set; }
}
