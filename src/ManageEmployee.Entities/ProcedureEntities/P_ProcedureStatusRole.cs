using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_ProcedureStatusRole
{
    public int Id { get; set; }
    public int P_ProcedureStatusId { get; set; }
    public int P_ProcedureId { get; set; }
    public int? RoleId { get; set; }
    public int? UserId { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
}
