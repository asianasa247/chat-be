using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.InOutEntities;

public class ProcedureChangeShift : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int? FromUserId { get; set; }
    public int? ToUserId { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    [StringLength(500)]
    public string? Note { get; set; }
}