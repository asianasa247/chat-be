using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_ProcedureStatusStep
{
    public int Id { get; set; }
    public int P_ProcedureId { get; set; }
    public int? ProcedureStatusIdFrom { get; set; }
    public int? ProcedureStatusIdTo { get; set; }
    public int? ProcedureConditionId { get; set; }
    public string? ProcedureConditionCode { get; set; }
    public bool IsFinish { get; set; }
    public bool IsInit { get; set; }
    public int Order { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
}
