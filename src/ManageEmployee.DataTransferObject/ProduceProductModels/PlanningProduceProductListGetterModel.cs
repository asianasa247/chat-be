namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class PlanningProduceProductListGetterModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public bool IsCanceled { get; set; }
    public string? ProcedureStatusName { get; set; }
}
