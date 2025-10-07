namespace ManageEmployee.DataTransferObject.SupplyModels;

public class ExpenditurePlanGetListModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
}
