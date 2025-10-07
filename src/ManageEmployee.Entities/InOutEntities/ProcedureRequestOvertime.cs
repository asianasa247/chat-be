using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.InOutEntities;

public class ProcedureRequestOvertime : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    public string? UserIdStr { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public int SymbolId { get; set; }
    public double Coefficient { get; set; }
}