namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class PlanningProduceProductPagingModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public double Quantity { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsFinished { get; set; }
    public bool IsDone { get; set; }
    public bool IsCanceled { get; set; }
    public bool ShoulDelete { get; set; }
    public bool ShoulNotAccept { get; set; }
    public int UserCreated { get; set; } = 0;
    public string? Code { get; set; }

}
