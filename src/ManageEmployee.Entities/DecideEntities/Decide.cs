using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.DecideEntities;

public class Decide : BaseEntity
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Code { get; set; }
    public int? Type { get; set; } = 0;
    public int? EmployeesId { get; set; }
    [StringLength(255)]
    public string? EmployeesName { get; set; }
    public int? DecideTypeId { get; set; }
    [StringLength(255)]
    public string? DecideTypeName { get; set; }
    public DateTime? Date { get; set; }
    [StringLength(255)]
    public string? Description { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
    [StringLength(500)]
    public string? FileUrl { get; set; }
    [StringLength(255)]
    public string? FileName { get; set; }
}
