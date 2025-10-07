namespace ManageEmployee.Entities.SupplyEntities;

public class ExpenditurePlanDetail
{
    public int Id { get; set; }
    public int ExpenditurePlanId { get; set; }
    public int FromProcedureId { get; set; }
    public string? FromProcedureCode { get; set; }
    public string? FromProcedureNote { get; set; }
    public string? FromTableName { get; set; }
    public string? Note { get; set; }
    public double? ExpenditurePlanAmount { get; set; }
    public double? ApproveAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? FileStr { get; set; }
    public string? FileStatusStr { get; set; }
    public bool IsApply { get; set; }
    public string? Status { get; set; }
}
