using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SupplyEntities;

public class AdvancePayment : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public double Amount { get; set; }
    public string? Note { get; set; }
    public string? FileStr { get; set; }
    public string? SettlementFileStr { get; set; }
    public bool IsDone { get; set; }
    public bool IsImmediate { get; set; }
    public DateTime DatePayment { get; set; }
}
