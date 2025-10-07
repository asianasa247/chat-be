namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class WarehouseProduceProductModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public List<WarehouseProduceProductDetailModel>? Items { get; set; }
}
