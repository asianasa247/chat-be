using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.StationeryEntities;

public class StationeryExport : BaseEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    [StringLength(36)]
    public string? ProcedureNumber { get; set; }
    public int DepartmentId { get; set; }
    public int UserId { get; set; }
}
