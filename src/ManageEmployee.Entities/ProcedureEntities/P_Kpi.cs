using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_Kpi : BaseEntity
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? ProcedureNumber { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
    public int Month { get; set; }
    public int? DepartmentId { get; set; }
    public int? P_ProcedureStatusId { get; set; }
    [StringLength(255)]
    public string? P_ProcedureStatusName { get; set; }
    public bool IsFinish { get; set; }
}
