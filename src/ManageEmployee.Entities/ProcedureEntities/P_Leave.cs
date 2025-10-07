using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_Leave : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
    [StringLength(1000)]
    public string? Reason { get; set; }
    public DateTime Fromdt { get; set; }
    public DateTime Todt { get; set; }
    public bool IsLicensed { get; set; }
    public int UserId { get; set; }
}
