using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class PlanningProduceProductGetDetailModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public FileDetailModel? FileDelivered { get; set; }
    public bool ShouldApproveGatePass { get; set; }
    public List<PlanningProduceProductCarGetDetailModel>? Items { get; set; }
}
