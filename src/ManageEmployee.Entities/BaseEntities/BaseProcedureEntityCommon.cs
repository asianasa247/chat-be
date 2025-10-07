using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.BaseEntities;

public class BaseProcedureEntityCommon : BaseEntityCommon
{
    public bool IsFinished { get; set; }
    [StringLength(255)]
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    [StringLength(255)]
    public string? ProcedureStatusName { get; set; }
    public string? NoteNotAccept { get; set; }
    [MaxLength(255)]
    public string? Code { get; set; }
}