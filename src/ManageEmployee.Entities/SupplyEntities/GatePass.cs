using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SupplyEntities;

public class GatePass : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public string? CarName { get; set; }
    public DateTime Date { get; set; }
    public string? Local { get; set; }
    public string? Content { get; set; }
    public string? Note { get; set; }
    public bool IsSpecial { get; set; }
    public string? FileStr { get; set; }
}