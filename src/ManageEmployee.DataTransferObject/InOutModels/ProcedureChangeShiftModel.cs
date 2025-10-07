namespace ManageEmployee.DataTransferObject.InOutModels;

public class ProcedureChangeShiftModel
{
    public int Id { get; set; }

    public string? ProcedureNumber { get; set; }

    public int? FromUserId { get; set; }
    public int? ToUserId { get; set; }
    public int? ProcedureStatusId { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }

    public string? ProcedureStatusName { get; set; }

    public string? Note { get; set; }

    public bool IsFinish { get; set; }
}