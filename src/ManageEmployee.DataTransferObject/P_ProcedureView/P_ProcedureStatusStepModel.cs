namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_ProcedureStatusStepModel
{
    public int? ProcedureStatusIdFrom { get; set; }
    public int? ProcedureStatusIdTo { get; set; }
    public int Order { get; set; }
    public string? Note { get; set; }
    public int? ProcedureConditionId { get; set; }
}
