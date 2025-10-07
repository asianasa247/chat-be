namespace ManageEmployee.DataTransferObject.SupplyModels;

public class RequestEquipmentOrderPagingModel
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public int UserId { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsFinished { get; set; }
    public bool ShoulDelete { get; set; }
    public bool ShoulNotAccept { get; set; }
}
