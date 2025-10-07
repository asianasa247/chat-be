namespace ManageEmployee.DataTransferObject.InOutModels;

public class ProcedureRequestOvertimePagingModel
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsFinished { get; set; }
    public int SymbolName { get; set; }
    public double Coefficient { get; set; }
}