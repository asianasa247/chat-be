using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.LedgerEntities;

public class LedgerProcedureProduct : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Type { get; set; }
    public string? Note { get; set; }
    public string? FileStr { get; set; }
}
