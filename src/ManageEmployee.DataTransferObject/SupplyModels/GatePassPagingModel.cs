namespace ManageEmployee.DataTransferObject.SupplyModels;

public class GatePassPagingModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Local { get; set; }
    public string? Content { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public string? CarName { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsSpecial { get; set; }
    public bool ShoulDelete { get; set; }
    public bool ShoulNotAccept { get; set; }
}