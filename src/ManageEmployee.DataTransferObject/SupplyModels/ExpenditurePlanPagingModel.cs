namespace ManageEmployee.DataTransferObject.SupplyModels;

public class ExpenditurePlanPagingModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }
    public double? ExpenditurePlanAmount { get; set; }
    public double? ApproveAmount { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool ShoulDelete { get; set; }
    public bool ShoulNotAccept { get; set; }
}