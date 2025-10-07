namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class ManufactureOrderGetDetailModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public int UserId { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public string? CanceledNote { get; set; }
    public List<ManufactureOrderGoodGetDetailModel>? Items { get; set; }
}
