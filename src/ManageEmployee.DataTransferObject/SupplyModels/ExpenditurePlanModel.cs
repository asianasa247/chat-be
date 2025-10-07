namespace ManageEmployee.DataTransferObject.SupplyModels;
public class ExpenditurePlanModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public int UserId { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public List<ExpenditurePlanDetailModel>? Items { get; set; }

}
