namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class ProcedureStatusModelResponse
{
    public int Id { get; set; }
    public string? ProcedureConditionCode { get; set; }
    public string? P_StatusName { get; set; }
    public bool IsFinish { get; set; }
}
